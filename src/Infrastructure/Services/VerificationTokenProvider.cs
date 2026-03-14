using Infrastructure.Interfaces;
using Infrastructure.Settings;
using System.Security.Cryptography;

namespace Infrastructure.Services
{
    public class VerificationTokenProvider : IVerificationTokenProvider
    {
        private readonly IRedisCacheService _redisCacheService;
        private readonly TokenCacheSettings _settings;

        public async Task<int> GenerateToken(string identifier, CancellationToken ct)
        {
            var token = RandomNumberGenerator.GetInt32(100_000, 999_999);
            await _redisCacheService.SetAsync(identifier, token,
                TimeSpan.FromMinutes(_settings.TokenExpirationMinutes), ct);
            return token;
        }

        public async Task<bool> ValidateToken(string identifier, string token, CancellationToken ct)
        {
            var storedToken = await _redisCacheService.GetAsync<string>(identifier, ct);
            return storedToken == token;
        }
    }
}
