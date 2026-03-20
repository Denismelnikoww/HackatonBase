using Application.DTO;

namespace Application.Interfaces;

public interface IApiService
{
    Task<ApiKeyDto> Generate(string name, Guid userId, CancellationToken ct);
    Task Revoke(Guid id, Guid userid, CancellationToken ct);
    Task<IEnumerable<ApiKeyDto>> GetKeys(Guid userId, CancellationToken ct);
}