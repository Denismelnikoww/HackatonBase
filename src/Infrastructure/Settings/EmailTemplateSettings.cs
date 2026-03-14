using static Infrastructure.Email.EmailTemplateBuilder;

namespace Infrastructure.Email
{
    public class EmailTemplateSettings
    {
        public string RecourcesPath { get; set; }
        public string EmailTemplateWithButtonFileName { get; set; }


        public EmailTemplateSettings()
        {
            if (string.IsNullOrEmpty(RecourcesPath))
                throw new InvalidOperationException("RecourcesPath не может быть пустым");

            if (string.IsNullOrEmpty(EmailTemplateWithButtonFileName))
                throw new InvalidOperationException("EmailTemplateWithButtonFileName не может быть пустым");

            var emailTemplate = Path.Combine(RecourcesPath, EmailTemplateWithButtonFileName);

            if (!File.Exists(emailTemplate))
                throw new FileNotFoundException($"HTML шаблон не найден по пути: {emailTemplate}");

            foreach (EmailType emailType in Enum.GetValues(typeof(EmailType)))
            {
                ValidateEmailTypeFiles(emailType);
            }
        }

        private void ValidateEmailTypeFiles(EmailType emailType)
        {
            var typeStr = emailType.ToString();
            var missingFiles = new List<string>();

            var titlePath = Path.Combine(RecourcesPath, $"TITLE-{typeStr}.txt");
            if (!File.Exists(titlePath))
                missingFiles.Add($"TITLE-{typeStr}.txt");

            var descriptionPath = Path.Combine(RecourcesPath, $"DESCRIPTION-{typeStr}.txt");
            if (!File.Exists(descriptionPath))
                missingFiles.Add($"DESCRIPTION-{typeStr}.txt");

            var buttonPath = Path.Combine(RecourcesPath, $"BUTTON-{typeStr}.txt");
            if (!File.Exists(buttonPath))
                missingFiles.Add($"BUTTON-{typeStr}.txt");

            if (missingFiles.Any())
            {
                throw new FileNotFoundException(
                    $"Для типа письма '{typeStr}' отсутствуют следующие файлы в папке " +
                    $"{RecourcesPath}: {string.Join(", ", missingFiles)}");
            }
        }
    }
}
