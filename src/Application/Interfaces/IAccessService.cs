using Domain.Models;

namespace Application.Interfaces;

public interface IAccessService
{
    Task<AccessStatus> CheckAcess(Guid qrId, Guid terminalId, CancellationToken ct = default);
}