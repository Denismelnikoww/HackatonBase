using Application.DTO;

namespace Application.Interfaces;

public interface IModeratorService
{
    Task<UserWithTerminalsDto> GetUser(Guid userId, CancellationToken ct = default);
    Task ChangeAccess(Guid userId, CancellationToken ct = default);
    Task<PagedResult<UserSmallDto>> GetUsers(int take, int skip, CancellationToken ct = default);
    Task SetTerminals(IEnumerable<Guid> terminalId, Guid userId, CancellationToken ct = default);
    Task<PagedResult<TerminalDto>> GetTerminals(Guid userId, int take, int skip, CancellationToken ct = default);
}