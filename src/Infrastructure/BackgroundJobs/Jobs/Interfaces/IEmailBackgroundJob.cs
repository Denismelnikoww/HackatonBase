namespace Infrastructure.BackgroundJobs.Jobs.Interfaces
{
    public interface IEmailBackgroundJob
    {
        Task ProcessEmailSendingAsync(string recipientEmail, string subject, string htmlText, string smtpHost, int smtpPort, string senderEmail, string senderPassword, string senderName, bool useSsl, int timeoutSeconds, int maxRetryAttempts, int retryDelaySeconds);
    }
}
