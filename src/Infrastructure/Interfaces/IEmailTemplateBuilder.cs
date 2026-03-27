using Infrastructure.Email;

namespace Infrastructure.Interfaces;

public interface IEmailTemplateBuilder
{
    string BuildEmail(EmailData emailData);
    string BuildEmailConfirmation(string code, int expiryTime = 30);
    string BuildResetPasswordEmail(string code, int expiryTime = 30);
    EmailData LoadEmailData(EmailType emailType, string code, int expiryTime = 30);
}