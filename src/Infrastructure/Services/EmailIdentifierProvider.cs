using Infrastructure.Interfaces;

namespace Infrastructure.Services
{
    public class EmailIdentifierProvider : IEmailIdentifierProvider
    {
        private readonly IRedisCacheService _redisCacheService;
        private readonly EmailCacheSettings _settings;

        public async Task<string> CreateIdentifier(string email, CancellationToken ct)
        {
            var id = Guid.NewGuid().ToString();
            await _redisCacheService.SetAsync(id, email,
                TimeSpan.FromMinutes(_settings.EmailExpirationSettings), ct);
            return id;
        }

        public async Task<string> GetEmail(string emailId, CancellationToken ct)
            => await _redisCacheService.GetAsync<string>(emailId, ct);
    }
}
