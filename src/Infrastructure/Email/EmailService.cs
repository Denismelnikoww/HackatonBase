using Infrastructure.BackgroundJobs;
using Infrastructure.Interfaces;
using Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly.Retry;

namespace Infrastructure.Email
{


    public class EmailService : IEmailService
    {
        private readonly SmtpOptions _options;
        private readonly ILogger<EmailService> _logger;
        private readonly IBackgroundJobService _backgroundJobService;

        public EmailService(
            IOptions<SmtpOptions> options,
            ILogger<EmailService> logger,
            IBackgroundJobService backgroundJobService)
        {
            _logger = logger;
            _options = options.Value;
            _backgroundJobService = backgroundJobService;
        }

        public async Task SendAsync(string email, string subject, string text, CancellationToken ct = default)
        {
            // Помещаем задачу в очередь. Вся логика повтора теперь внутри задачи.
            _backgroundJobService.Enqueue<IEmailBackgroundJob>(x => x.ProcessEmailSendingAsync(
                recipientEmail: email,
                subject: subject,
                htmlText: text,
                smtpHost: _options.Host,
                smtpPort: _options.Port,
                senderEmail: _options.Email,
                senderPassword: _options.Password, // Убедитесь, что пароль безопасно передается и хранится
                senderName: _options.Name,
                useSsl: _options.UseSsl,
                timeoutSeconds: _options.TimeoutSeconds,
                maxRetryAttempts: _options.MaxRetryAttempts, // Передаем настройки из Options
                retryDelaySeconds: _options.RetryDelaySeconds
            ));

            _logger.LogInformation("Email job enqueued for {Email}", email);
            // Метод возвращает управление сразу после помещения задачи в очередь
        }
    }
}
