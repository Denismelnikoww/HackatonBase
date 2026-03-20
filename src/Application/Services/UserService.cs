using Application.DTO;
using Application.Interfaces;
using Domain.Exceptions;
using Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class UserService(AppDbContext context) : IUserService
    {
        public async Task<UserInfo?> GetInfo(Guid userId, CancellationToken ct = default)
        {
            var userInfo = await context.Users.AsNoTracking()
                .Where(u => u.Id == userId)
                .Select(u => new UserInfo
                {
                    Name = u.Name,
                    Role = u.Role
                })
                .FirstOrDefaultAsync(ct);

            if (userInfo == null) throw new NotFoundException("Такого пользователя не существует");

            return userInfo;
        }
    }
}