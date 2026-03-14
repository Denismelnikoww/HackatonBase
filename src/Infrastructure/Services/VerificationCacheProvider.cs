using Infrastructure.Interfaces;
using Infrastructure.Options;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace Infrastructure.Services
{
    public class VerificationCacheProvider : IVerificationCacheProvider
    {
        private readonly IRedisCacheService _redisCacheService;
        private readonly VerificationCacheOptions _options;

        public VerificationCacheProvider(IRedisCacheService redisCacheService,
            IOptions<VerificationCacheOptions> options)
        {
            _redisCacheService = redisCacheService;
            _options = options.Value;
        }

        public async Task<int> GenerateToken(string key, CancellationToken ct)
        {
            var token = RandomNumberGenerator.GetInt32(100_000, 999_999);
            await _redisCacheService.SetAsync(key, token,
                TimeSpan.FromMinutes(_options.TokenExpirationMinutes), ct);
            return token;
        }

        public async Task<bool> ValidateToken(string key, string token, CancellationToken ct)
        {
            var storedToken = await _redisCacheService.GetAsync<string>(key, ct);
            return storedToken == token;
        }

        public async Task<string> CreateEmailIdentifier(string email, CancellationToken ct)
        {
            var id = Guid.NewGuid().ToString();
            await _redisCacheService.SetAsync(id, email,
                TimeSpan.FromMinutes(_options.EmailExpirationMinutes), ct);
            return id;
        }

        public async Task<string> GetEmailByKey(string key, CancellationToken ct)
            => await _redisCacheService.GetAsync<string>(key, ct);
    }
}
