using Infrastructure.Interfaces;
using Infrastructure.Options;
using Microsoft.Extensions.Options;
using System.Text;

namespace Infrastructure.Email
{
    public partial class EmailTemplateBuilder : IEmailTemplateBuilder
    {
        private readonly EmailTemplateOptions _options;
        private string _cachedTemplate;

        public EmailTemplateBuilder(IOptions<EmailTemplateOptions> options)
        {
            _options = options.Value;
        }

        public EmailData LoadEmailData(EmailType emailType, string link, int expiryTime = 30)
        {
            var typeStr = emailType.ToString();

            var titlePath = Path.Combine(_options.ResourcesPath, $"TITLE-{typeStr}.txt");
            var descriptionPath = Path.Combine(_options.ResourcesPath, $"DESCRIPTION-{typeStr}.txt");
            var buttonPath = Path.Combine(_options.ResourcesPath, $"BUTTON-{typeStr}.txt");

            string title = File.ReadAllText(titlePath, Encoding.UTF8).Trim();
            string description = File.ReadAllText(descriptionPath, Encoding.UTF8).Trim();
            string button = File.ReadAllText(buttonPath, Encoding.UTF8).Trim();

            return new EmailData
            {
                Title = title,
                Description = description,
                Link = link,
                ButtonText = button,
                ExpiryTime = expiryTime
            };
        }

        public string BuildEmail(EmailData emailData)
        {
            if (emailData == null)
                throw new ArgumentNullException(nameof(emailData));

            string result = _cachedTemplate
                .Replace("{{TITLE}}", emailData.Title)
                .Replace("{{DESCRIPTION}}", emailData.Description)
                .Replace("{{LINK}}", emailData.Link)
                .Replace("{{BUTTON_TEXT}}", emailData.ButtonText)
                .Replace("{{EXPIRY_TIME}}", emailData.ExpiryTime.ToString());

            return result;
        }

        public string BuildEmailConfirmation(string confirmationLink, int expiryTime = 30)
        {
            var data = LoadEmailData(EmailType.EmailConfirm, confirmationLink, expiryTime);
            return BuildEmail(data);
        }

        public string BuildResetPasswordEmail(string resetLink, int expiryTime = 30)
        {
            var data = LoadEmailData(EmailType.ResetPassword, resetLink, expiryTime);
            return BuildEmail(data);
        }

    }
}
