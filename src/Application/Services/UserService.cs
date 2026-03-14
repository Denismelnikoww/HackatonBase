using Application.DTO;
using Application.Interfaces;
using Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using ResultSharp.Core;
using ResultSharp.Errors;

namespace Application.Services
{
    public class UserService(UserDbContext context) : IUserService
    {
        public async Task<Result> UpdateActivity(Guid sessionId, CancellationToken ct = default)
        {
            var session = await context.UserSessions
                .FirstOrDefaultAsync(x => x.Id == sessionId, ct);

            if (session == null)
                return Error.NotFound("Такой сессии не существует");

            session.LastActivity = DateTime.UtcNow;
            await context.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result<UserInfo?>> GetInfo(Guid userId, CancellationToken ct = default)
        {
            var userInfo = await context.Users.AsNoTracking()
                .Where(u => u.Id == userId)
                .Select(u => new UserInfo
                {
                    Name = u.Name,
                    Role = u.Role
                })
                .FirstOrDefaultAsync(ct);

            if (userInfo == null)
                return Error.NotFound("Такого пользователя не существует");

            return userInfo;
        }
    }
}