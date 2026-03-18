namespace Infrastructure.Email
{
    public interface IEmailBackgroundJob
    {
        // Передаем только конкретные данные, необходимые для отправки
        // Это делает сериализацию более надежной
        Task ProcessEmailSendingAsync(string recipientEmail, string subject, string htmlText, string smtpHost, int smtpPort, string senderEmail, string senderPassword, string senderName, bool useSsl, int timeoutSeconds, int maxRetryAttempts, int retryDelaySeconds);
    }
}
