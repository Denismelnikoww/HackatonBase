using Infrastructure.Email;

namespace Infrastructure.Interfaces
{
    public interface IEmailTemplateBuilder
    {
        string BuildEmail(EmailTemplateBuilder.EmailData emailData);
        string BuildEmailConfirmation(string confirmationLink, int expiryTime = 30);
        string BuildResetPasswordEmail(string resetLink, int expiryTime = 30);
        EmailTemplateBuilder.EmailData LoadEmailData(EmailTemplateBuilder.EmailType emailType, string link, int expiryTime = 30);
    }
}