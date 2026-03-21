using Application.DTO;

namespace Application.Interfaces;

public interface IUploadService
{
    Task UploadUsersAsync(List<UserLargeDto> users, CancellationToken ct = default);
    Task UploadTerminalsAsync(List<TerminalDto> terminals, CancellationToken ct = default);
}