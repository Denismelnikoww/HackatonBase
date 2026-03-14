namespace Infrastructure.Options
{
    public class VerificationCacheOptions
    {
        public int TokenExpirationMinutes { get; set; } = 30;
        public int EmailExpirationMinutes { get; set; } = 30;
    }
}
