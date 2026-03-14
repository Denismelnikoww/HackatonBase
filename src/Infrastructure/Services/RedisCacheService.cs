using Infrastructure.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Infrastructure.Services
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<RedisCacheService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public RedisCacheService(IDistributedCache cache, ILogger<RedisCacheService> logger)
        {
            _cache = cache;
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<T> GetAsync<T>(string key, CancellationToken ct = default)
        {
            var bytes = await _cache.GetAsync(key, ct);
            if (bytes == null)
                return default;

            var json = System.Text.Encoding.UTF8.GetString(bytes);
            return JsonSerializer.Deserialize<T>(json, _jsonOptions);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null,
            CancellationToken ct = default)
        {
            var options = new DistributedCacheEntryOptions();

            if (expiry.HasValue)
                options.SetAbsoluteExpiration(expiry.Value);
            else
                options.SetSlidingExpiration(TimeSpan.FromMinutes(20));

            var json = JsonSerializer.Serialize(value, _jsonOptions);
            var bytes = System.Text.Encoding.UTF8.GetBytes(json);

            await _cache.SetAsync(key, bytes, options, ct);

            _logger.LogDebug("Кэш установлен для ключа: {Key}", key);
        }

        public async Task RemoveAsync(string key, CancellationToken ct)
        {
            await _cache.RemoveAsync(key, ct);
            _logger.LogDebug("Кэш удален для ключа: {Key}", key);
        }

        public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory,
            TimeSpan? expiry = null, CancellationToken ct = default)
        {
            var cached = await GetAsync<T>(key);
            if (cached != null)
            {
                return cached;
            }

            var value = await factory();
            if (value != null)
            {
                await SetAsync(key, value, expiry, ct);
            }

            return value;
        }

        public async Task RefreshAsync(string key, CancellationToken ct)
        {
            await _cache.RefreshAsync(key, ct);
        }
    }
}
