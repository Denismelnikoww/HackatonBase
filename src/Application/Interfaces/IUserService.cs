using Application.DTO;
using ResultSharp.Core;

namespace Application.Interfaces
{
    public interface IUserService
    {
        Task<Result<UserInfo?>> GetInfo(Guid userId, CancellationToken ct = default);
        Task<Result> UpdateActivity(Guid id, CancellationToken ct = default);
    }
}