using Microsoft.Extensions.Options;

namespace Infrastructure.Options.Validators
{
    public class MistralOptionsValidator : IValidateOptions<MistralOptions>
    {
        public ValidateOptionsResult Validate(string name, MistralOptions options)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(options.ApiKey))
                errors.Add("API ключ Mistral обязателен");

            if (string.IsNullOrWhiteSpace(options.BaseUrl))
                errors.Add("URL Mistral обязателен");

            if (errors.Any())
                return ValidateOptionsResult.Fail(string.Join("; ", errors));

            return ValidateOptionsResult.Success;
        }

    }
}

