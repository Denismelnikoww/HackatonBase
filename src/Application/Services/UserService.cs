using Application.Interfaces;
using Domain.Models.User;
using Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly UserDbContext _context;
        private readonly DbSet<Session> _session;
        private readonly DbSet<User> _users;

        public UserService(UserDbContext context)
        {
            _context = context;
            _session = _context.Set<Session>();
        }

        public async Task UpdateActivity(Guid sessionId)
        {
            var session = await _session.FirstOrDefaultAsync(x => x.Id == sessionId);
            if (session == null)
                return;
            session.LastActivity = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        public async Task<UserInfo?> GetInfo(Guid userId, CancellationToken ct = default)
        {
            return await _users.AsNoTracking()
                .Select(u => new UserInfo
                {
                    Name = u.Name,
                    Role = u.Role
                })
                .FirstOrDefaultAsync();
        }
    }
}