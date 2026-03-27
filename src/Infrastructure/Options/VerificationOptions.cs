namespace Infrastructure.Options;

public class VerificationOptions
{
    public int EmailTokenExpirationMinutes { get; set; } = 30;
    public int EmailTokenLength { get; set; } = 4;

    public int PasswordTokenExpirationMinutes { get; set; } = 30;
    public int PasswordTokenLength { get; set; } = 4;
}
