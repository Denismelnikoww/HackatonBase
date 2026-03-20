using Microsoft.Extensions.Options;

namespace Infrastructure.Options.Validators;

public class QrOptionsValidator : IValidateOptions<QrOptions>
{
    public ValidateOptionsResult Validate(string name, QrOptions options)
    {
        var errors = new List<string>();

        if (options.ExpirationSeconds <= 0)
            errors.Add("EmailExpirationMinutes должен быть больше 0");

        return errors.Any()
            ? ValidateOptionsResult.Fail(string.Join("; ", errors))
            : ValidateOptionsResult.Success;
    }
}

