using Application.DTO;
using ResultSharp.Core;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        Task<Result<JwtTokens>> Login(string? login, string password, CancellationToken ct = default);
        Task<Result> Logout(Guid sessionId, CancellationToken ct = default);
        Task<Result<JwtTokens>> Refresh(string refreshToken, Guid userId, Guid sessionId, CancellationToken ct);
        Task<Result> Register(string name, string login, string password, string? email = null, CancellationToken ct = default);
    }
}
