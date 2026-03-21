using Application.DTO;

namespace Application.Interfaces
{
    public interface IImportService
    {
        Task<PagedResult<EntryDto>> GetEntries(int take, int skip, CancellationToken ct);
        Task<PagedResult<TerminalDto>> GetTerminals(int take, int skip, CancellationToken ct);
        Task<PagedResult<UserLargeDto>> GetUsers(int take, int skip, CancellationToken ct);
    }
}