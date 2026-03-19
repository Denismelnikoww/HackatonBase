using Microsoft.Extensions.Options;

namespace Infrastructure.Options.Validators
{
    public class VerificationCacheOptionsValidator : IValidateOptions<VerificationOptions>
    {
        public ValidateOptionsResult Validate(string name, VerificationOptions options)
        {
            var errors = new List<string>();

            if (options.TokenExpirationMinutes <= 0)
                errors.Add("TokenExpirationMinutes должен быть больше 0");

            if (options.EmailExpirationMinutes <= 0)
                errors.Add("EmailExpirationMinutes должен быть больше 0");

            return errors.Any()
                ? ValidateOptionsResult.Fail(string.Join("; ", errors))
                : ValidateOptionsResult.Success;
        }
    }
}
