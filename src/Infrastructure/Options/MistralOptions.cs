namespace Infrastructure.Options
{
    public class MistralOptions
    {
        public string ApiKey { get; set; }
        public string BaseUrl { get; set; } = "https://api.mistral.ai/v1/";
        public int RetryCount { get; set; } = 3;
        public string BaseAgentId { get; set; }
        public int BaseAgentVersion { get; set; } = 1;
    }
}
