using Application.DTO;

namespace Application.Interfaces
{
    public interface IUserService
    {
        Task<UserInfo?> GetInfo(Guid userId, CancellationToken ct = default);
    }
}