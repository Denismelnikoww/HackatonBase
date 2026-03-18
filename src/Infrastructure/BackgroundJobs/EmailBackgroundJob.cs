using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using MimeKit;
using Polly;

namespace Infrastructure.Email
{
    public class EmailBackgroundJob : IEmailBackgroundJob
    {
        private readonly ILogger<EmailBackgroundJob> _logger;

        public EmailBackgroundJob(ILogger<EmailBackgroundJob> logger)
        {
            _logger = logger;
        }

        public async Task ProcessEmailSendingAsync(
            string recipientEmail,
            string subject,
            string htmlText,
            string smtpHost,
            int smtpPort,
            string senderEmail,
            string senderPassword,
            string senderName,
            bool useSsl,
            int timeoutSeconds,
            int maxRetryAttempts,
            int retryDelaySeconds)
        {
            // Создаем политику повтора внутри задачи
            var retryPolicy = Policy
                .Handle<IOException>()
                .Or<TimeoutException>()
                .Or<SmtpCommandException>(ex =>
                    ex.StatusCode == SmtpStatusCode.ServiceClosingTransmissionChannel ||
                    ex.StatusCode == SmtpStatusCode.TransactionFailed)
                .Or<Exception>(ex =>
                    ex.Message.Contains("timed out", StringComparison.OrdinalIgnoreCase) ||
                    ex.Message.Contains("connection", StringComparison.OrdinalIgnoreCase) ||
                    ex.Message.Contains("network", StringComparison.OrdinalIgnoreCase))
                .WaitAndRetryAsync(
                    maxRetryAttempts,
                    retryAttempt => TimeSpan.FromSeconds(retryDelaySeconds * Math.Pow(2, retryAttempt - 1)), // Экспоненциальная задержка
                    onRetry: (outcome, timespan, retryCount, context) =>
                    {
                        _logger.LogWarning(
                            outcome.Exception,
                            "Retry {RetryCount} after {Delay}s for sending email to {Recipient}. Exception: {ExceptionMessage}",
                            retryCount, timespan.TotalSeconds, recipientEmail, outcome.Exception?.Message);
                    });

            // Выполняем отправку с политикой повтора
            await retryPolicy.ExecuteAsync(async (ct) =>
            {
                await SendEmailCoreAsync(recipientEmail, subject, htmlText, smtpHost, smtpPort, senderEmail, senderPassword, senderName, useSsl, timeoutSeconds, ct);
            }, CancellationToken.None); // Используем токен от Hangfire, если нужно, или CancellationToken.None если внутренняя логика не использует отмену
        }

        private async Task SendEmailCoreAsync(
            string recipientEmail,
            string subject,
            string htmlText,
            string smtpHost,
            int smtpPort,
            string senderEmail,
            string senderPassword,
            string senderName,
            bool useSsl,
            int timeoutSeconds,
            CancellationToken ct)
        {
            using var client = new SmtpClient();
            using var message = new MimeMessage();

            message.From.Add(new MailboxAddress(senderName, senderEmail));
            message.To.Add(new MailboxAddress("", recipientEmail)); // Имя получателя можно передать отдельно, если нужно
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = htmlText };

            client.Timeout = timeoutSeconds * 1000; // Устанавливаем таймаут

            try
            {
                await client.ConnectAsync(smtpHost, smtpPort, useSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTlsWhenAvailable, ct);
                await client.AuthenticateAsync(senderEmail, senderPassword, ct);
                await client.SendAsync(message, ct);

                _logger.LogInformation("Email successfully sent to {RecipientEmail}", recipientEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {RecipientEmail} during execution attempt.", recipientEmail);
                // Исключение выбрасывается, и Polly обработает его в соответствии с политикой
                throw;
            }
            finally
            {
                if (client.IsConnected)
                {
                    await client.DisconnectAsync(true, ct);
                }
                // SmtpClient и MimeMessage реализуют IDisposable, using гарантирует освобождение
            }
        }
    }
}
