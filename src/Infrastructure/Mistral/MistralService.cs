using Infrastructure.Interfaces;
using Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Infrastructure.Mistral
{
    public class MistralService : IMistralService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<MistralService> _logger;
        private readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;
        private readonly MistralOptions _options;

        public MistralService(
            IHttpClientFactory httpClientFactory,
            IOptions<MistralOptions> options,
            ILogger<MistralService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _options = options.Value;
            _logger = logger;

            _retryPolicy = Policy<HttpResponseMessage>
                .Handle<HttpRequestException>()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.TooManyRequests
                              || (int)msg.StatusCode >= 500)
                .WaitAndRetryAsync(
                    retryCount: _options.RetryCount,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (outcome, timespan, retryCount, context) =>
                    {
                        _logger.LogWarning("Retry {RetryCount} after {Seconds}s due to: {Error}",
                            retryCount, timespan.Seconds,
                            outcome.Exception?.Message ?? outcome.Result.StatusCode.ToString());
                    });
        }

        public async Task<MistralResponse> CreateConversationAsync(string message)
            => await CreateConversationAsync(_options.BaseAgentId,
                _options.BaseAgentVersion, message.ToMessages());

        public async Task<MistralResponse> CreateConversationAsync(
            string agentId,
            int agentVersion,
            IEnumerable<Message> inputs)
        {
            var httpClient = _httpClientFactory.CreateClient("Mistral");

            var request = new MistralRequest
            {
                AgentId = agentId,
                AgentVersion = agentVersion,
                Inputs = inputs
            };

            var json = JsonSerializer.Serialize(request, _jsonSerializerOptions);
            _logger.LogDebug("Request: {Json}", json);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _retryPolicy.ExecuteAsync(async () =>
            {
                return await httpClient.PostAsync("conversations", content);
            });

            var responseJson = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Mistral API error: {response.StatusCode} - {responseJson}");
            }

            return JsonSerializer.Deserialize<MistralResponse>(responseJson, _jsonSerializerOptions);
        }
    }
}