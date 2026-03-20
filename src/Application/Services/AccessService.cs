using Infrastructure.DbContexts;
using Infrastructure.Interfaces;
using MailKit;

namespace Application.Services;

public class AccessService(AppDbContext context, IRedisCacheService redisCacheService)
{
    public string Validate(Guid qrToken, CancellationToken ct)
    {
        redisCacheService.GetAsync<Guid>(qrToken.ToString(), ct);
        return "";
    }
}