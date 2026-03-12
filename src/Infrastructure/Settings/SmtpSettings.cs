namespace Infrastructure.Settings
{
    public class SmtpSettings
    {
        public string Host { get; set; }
        public string Name { get; set; }
        public int Port { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public int MaxConcurrentConnections { get; set; }
        public int MaxRetryAttempts { get; set; } = 3;
        public int TimeoutSeconds { get; set; } = 60;
        public int RetryDelaySeconds { get; set; } = 5;
        public bool UseSsl { get; set; } = true;
    }
}