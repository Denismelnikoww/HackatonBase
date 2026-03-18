using System.Text.Json.Serialization;

namespace Infrastructure.Mistral
{
    public class MistralResponse
    {
        [JsonPropertyName("conversation_id")]
        public string ConversationId { get; set; }
        [JsonPropertyName("object")]
        public string Object { get; set; }
        [JsonPropertyName("usage")]
        public Usage Usage { get; set; }
        [JsonPropertyName("outputs")]
        public IEnumerable<Output> Outputs { get; set; }
    }
}
