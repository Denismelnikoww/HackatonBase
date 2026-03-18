using Infrastructure.Mistral;

namespace Infrastructure.Interfaces
{
    public interface IMistralService
    {
        Task<MistralResponse> CreateConversationAsync(string agentId, int agentVersion, IEnumerable<Message> inputs);
        Task<MistralResponse> CreateConversationAsync(string message);
    }
}