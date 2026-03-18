using System.Text.Json.Serialization;

namespace Infrastructure.Mistral
{
    public class Message
    {
        [JsonPropertyName("role")]
        public string Role { get; set; }
        [JsonPropertyName("content")]
        public string Content { get; set; }
    }

    public static class MessageExtensions
    {
        public static Message[] ToMessages(this string content, string role = "user")
        {
            return new[] { new Message { Role = role, Content = content } };
        }
    }

}
