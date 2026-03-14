using Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using System.Collections.Concurrent;
using MailKit.Security;
using Infrastructure.Interfaces;

namespace Infrastructure.Email
{
    public class EmailService(
        IOptions<SmtpOptions> options,
        ILogger<EmailService> logger) : IDisposable, IEmailService
    {
        private readonly SmtpOptions _options = options.Value;
        private readonly ConcurrentBag<SmtpClient> _clientPool = new();
        private readonly SemaphoreSlim _semaphore =
            new SemaphoreSlim(options.Value.MaxConcurrentConnections);

        private int _activeClients = 0;
        private bool _disposed;


        public async Task SendAsync(string email, string subject, string text, CancellationToken ct = default)
        {
            var attempt = 0;
            var maxAttempts = _options.MaxRetryAttempts;
            var delay = _options.RetryDelaySeconds;

            while (attempt < maxAttempts)
            {
                try
                {
                    await SendInternalAsync(email, subject, text, ct);
                    return;
                }
                catch (Exception ex) when (attempt < maxAttempts - 1)
                {
                    attempt++;

                    logger.LogWarning(ex,
                        "Failed to send email to {Email}, attempt {Attempt}/{MaxAttempts}. Retrying in {Delay}s",
                        email, attempt, maxAttempts, delay);

                    var waitTime = TimeSpan.FromSeconds(delay * Math.Pow(2, attempt - 1));
                    waitTime = TimeSpan.FromMilliseconds(waitTime.TotalMilliseconds);
                    await Task.Delay(waitTime, ct);
                }
            }
        }

        private async Task SendInternalAsync(string email, string subject, string text, CancellationToken ct)
        {
            await _semaphore.WaitAsync(ct);

            SmtpClient? client = null;
            var needReconnect = false;

            try
            {
                if (!_clientPool.TryTake(out client))
                {
                    client = await CreateClientAsync(ct);
                    Interlocked.Increment(ref _activeClients);
                }
                else
                {
                    if (!client.IsConnected || client.IsAuthenticated == false)
                    {
                        logger.LogDebug("Reconnecting stale client");
                        await ReconnectClientAsync(client, ct);
                    }
                }

                using var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_options.Name, _options.Email));
                message.To.Add(new MailboxAddress("", email));
                message.Subject = subject;
                message.Body = new TextPart("html") { Text = text };

                await client.SendAsync(message, ct);

                _clientPool.Add(client);
                client = null;

                logger.LogDebug("Email sent to {Email}", email);
            }
            catch (Exception ex)
            {
                if (IsConnectionError(ex))
                {
                    logger.LogWarning(ex, "Connection error, client will be disposed");
                    client?.Dispose();
                    client = null;
                }
                else
                {
                    try
                    {
                        _clientPool.Add(client!);
                        client = null;
                    }
                    catch
                    {
                        client?.Dispose();
                    }
                }
                throw;
            }
            finally
            {
                _semaphore.Release();
                if (client != null)
                {
                    client.Dispose();
                    Interlocked.Decrement(ref _activeClients);
                }
            }
        }

        private async Task<SmtpClient> CreateClientAsync(CancellationToken ct)
        {
            var client = new SmtpClient();

            try
            {
                await ConnectAndAuthClientAsync(client, ct);
                return client;
            }
            catch
            {
                client.Dispose();
                throw;
            }
        }

        private async Task ReconnectClientAsync(SmtpClient client, CancellationToken ct)
        {
            try
            {
                if (client.IsConnected)
                {
                    await client.DisconnectAsync(true, ct);
                }

                await ConnectAndAuthClientAsync(client, ct);
            }
            catch
            {
                throw;
            }
        }

        private async Task ConnectAndAuthClientAsync(SmtpClient client, CancellationToken ct)
        {
            client.Timeout = _options.TimeoutSeconds * 1000;

            await client.ConnectAsync(
                _options.Host,
                _options.Port,
                _options.UseSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls,
                ct);

            await client.AuthenticateAsync(
                _options.Email,
                _options.Password,
                ct);
        }

        private bool IsConnectionError(Exception ex)
        {
            return ex is IOException
                || ex is SmtpCommandException cmdEx && cmdEx.StatusCode == SmtpStatusCode.ServiceClosingTransmissionChannel
                || ex.Message.Contains("timed out", StringComparison.OrdinalIgnoreCase)
                || ex.Message.Contains("connection", StringComparison.OrdinalIgnoreCase);
        }

        public void Dispose()
        {
            if (_disposed) return;

            while (_clientPool.TryTake(out var client))
            {
                try
                {
                    if (client.IsConnected)
                        client.Disconnect(true);
                    client.Dispose();
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Error disposing SMTP client");
                }
            }

            _semaphore.Dispose();
            _disposed = true;
        }
    }
}