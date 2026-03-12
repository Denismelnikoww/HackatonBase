using Domain.Models.User;
using System.Security.Claims;

namespace Infrastructure.Interfaces
{
    public interface IJwtProvider
    {
        string GenerateAccessToken(Guid userId, Guid sessionId, Role role);
        string GenerateRefreshToken(Guid userId, Guid sessionId);
        ClaimsPrincipal? ValidateRefreshToken(string token);
    }
}
