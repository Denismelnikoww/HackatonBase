using Application.Interfaces;
using Infrastructure.Interfaces;
using Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace Application.Services;

public class QrService(
    IRedisCacheService redisCacheService,
    IOptions<QrOptions> qrOptions) : IQrService
{
    public async Task<Guid> Generate(Guid userId, CancellationToken ct = default)
    {
        var guid = Guid.NewGuid();

        redisCacheService.SetAsync(guid.ToString(),
            userId,
            TimeSpan.FromSeconds(qrOptions.Value.ExpirationSeconds),
            ct);

        return guid;
    }

    public async Task Revoke(Guid qrId, CancellationToken ct = default)
        => await redisCacheService.RemoveAsync(qrId.ToString(), ct);

    public async Task<Guid> Parse(Guid qrId, CancellationToken ct = default)
    {
        var guid = Guid.NewGuid();

        await redisCacheService.GetAsync<Guid>(guid.ToString(), ct);

        return guid;
    }
}