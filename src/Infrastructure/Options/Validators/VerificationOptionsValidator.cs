using Microsoft.Extensions.Options;

namespace Infrastructure.Options.Validators
{
    public class VerificationOptionsValidator : IValidateOptions<VerificationOptions>
    {
        public ValidateOptionsResult Validate(string name, VerificationOptions options)
        {
            var errors = new List<string>();

            if (options.EmailExpirationMinutes <= 0)
                errors.Add("EmailExpirationMinutes должен быть больше 0");

            if (string.IsNullOrEmpty(options.ConfirmEmailLink))
                errors.Add("ConfirmEmailLink обязателен");

            if (string.IsNullOrEmpty(options.ResetPasswordLink))
                errors.Add("ResetPasswordLink обязателен");

            return errors.Any()
                ? ValidateOptionsResult.Fail(string.Join("; ", errors))
                : ValidateOptionsResult.Success;
        }
    }
}
