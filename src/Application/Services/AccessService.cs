using Infrastructure.DbContexts;
using Infrastructure.Interfaces;

namespace Application.Services;

public class AccessService(AppDbContext context, IRedisCacheService redisCacheService)
{
    public string Validate(Guid qrToken)
    {
        return "";
    }
}