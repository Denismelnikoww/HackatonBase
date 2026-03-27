using Infrastructure.Interfaces;
using Infrastructure.Options;
using Microsoft.Extensions.Options;
using System.Text;

namespace Infrastructure.Email;

public partial class EmailTemplateBuilder : IEmailTemplateBuilder
{
    private readonly EmailTemplateOptions _options;
    private string _cachedTemplate;

    public EmailTemplateBuilder(IOptions<EmailTemplateOptions> options, EmailType type)
    {
        _options = options.Value;
        var templatePath = "";
        if (type == EmailType.EmailConfirm || type == EmailType.ResetPassword)
            templatePath = Path.Combine(_options.ResourcesPath, _options.EmailTemplateWithCodeFileName);
        _cachedTemplate = File.ReadAllText(templatePath, Encoding.UTF8);
    }

    public EmailData LoadEmailData(EmailType emailType, string code, int expiryTime = 30)
    {
        var typeStr = emailType.ToString();

        var titlePath = Path.Combine(_options.ResourcesPath, $"TITLE-{typeStr}.txt");
        var descriptionPath = Path.Combine(_options.ResourcesPath, $"DESCRIPTION-{typeStr}.txt");

        string title = File.ReadAllText(titlePath, Encoding.UTF8).Trim();
        string description = File.ReadAllText(descriptionPath, Encoding.UTF8).Trim();

        return new EmailData
        {
            Title = title,
            Description = description,
            Code = code,
            ExpiryTime = expiryTime
        };
    }

    public string BuildEmail(EmailData emailData)
        => _cachedTemplate.Replace("{{TITLE}}", emailData.Title)
            .Replace("{{DESCRIPTION}}", emailData.Description)
            .Replace("{{CODE}}", emailData.Code)
            .Replace("{{EXPIRY_TIME}}", emailData.ExpiryTime.ToString());


    public string BuildEmailConfirmation(string code, int expiryTime = 30)
    {
        var data = LoadEmailData(EmailType.EmailConfirm, code, expiryTime);
        return BuildEmail(data);
    }

    public string BuildResetPasswordEmail(string code, int expiryTime = 30)
    {
        var data = LoadEmailData(EmailType.ResetPassword, code, expiryTime);
        return BuildEmail(data);
    }
}
