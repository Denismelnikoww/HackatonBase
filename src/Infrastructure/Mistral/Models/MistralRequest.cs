using System.Text.Json.Serialization;

namespace Infrastructure.Mistral
{
    public class MistralRequest
    {
        [JsonPropertyName("agent_id")]
        public string AgentId { get; set; }
        [JsonPropertyName("agent_version")]
        public int AgentVersion { get; set; }
        [JsonPropertyName("inputs")]
        public IEnumerable<Message> Inputs { get; set; }
    }
}
