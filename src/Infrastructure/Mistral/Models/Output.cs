using System.Text.Json.Serialization;

namespace Infrastructure.Mistral
{
    public class Output
    {
        [JsonPropertyName("object")]
        public string Object { get; set; }
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }
        [JsonPropertyName("completed_at")]
        public DateTime CompletedAt { get; set; }
        [JsonPropertyName("agent_id")]
        public string AgentId { get; set; }
        [JsonPropertyName("model")]
        public string Model { get; set; }
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("role")]
        public string Role { get; set; }
        [JsonPropertyName("content")]
        public string Content { get; set; }
    }
}
