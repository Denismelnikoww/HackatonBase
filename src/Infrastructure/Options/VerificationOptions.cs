namespace Infrastructure.Options
{
    public class VerificationOptions
    {
        public int EmailExpirationMinutes { get; set; } = 30;
        public string ConfirmEmailLink { get; set; }
        public string ResetPasswordLink { get; set; }
    }
}
