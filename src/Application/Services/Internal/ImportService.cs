using Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using Application.DTO;
using Application.Interfaces;

namespace Application.Services.Internal
{
    public class ImportService(AppDbContext context) : IImportService
    {
        public async Task<PagedResult<UserLargeDto>> GetUsers(int take, int skip, CancellationToken ct)
        {
            var users = await context.Users.AsNoTracking()
                .Where(u => !u.IsDeleted)
                .Skip(skip)
                .Take(take)
                .Select(u => new UserLargeDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Login = u.Login,
                    Email = u.Email,
                    IsEmailConfirmed = u.IsEmailConfirmed,
                    Role = u.Role,
                    RegistrationDate = u.RegistrationDate,
                    IsBanned = u.IsBanned,
                    EntryAccess = u.EntryAccess
                })
                .ToListAsync(ct);

            var count = await context.Users.AsNoTracking()
                .Where(u => !u.IsDeleted)
                .CountAsync(ct);

            return new PagedResult<UserLargeDto>
            {
                Items = users,
                Take = take,
                Skip = skip,
                Total = count
            };
        }

        public async Task<PagedResult<TerminalDto>> GetTerminals(int take, int skip, CancellationToken ct)
        {
            var terminals = await context.Terminals.AsNoTracking()
                .Where(u => !u.IsDeleted)
                .Skip(skip)
                .Take(take)
                .Select(t => new TerminalDto
                {
                    Id = t.Id,
                    Name = t.Name,
                })
                .ToListAsync(ct);

            var count = await context.Terminals.AsNoTracking()
                .Where(u => !u.IsDeleted)
                .CountAsync(ct);

            return new PagedResult<TerminalDto>
            {
                Items = terminals,
                Take = take,
                Skip = skip,
                Total = terminals.Count
            };
        }

        public async Task<PagedResult<EntryDto>> GetEntries(int take, int skip, CancellationToken ct)
        {
            var entries = await context.Entries.AsNoTracking()
                .Skip(skip)
                .Take(take)
                .Select(t => new EntryDto
                {
                    Id = t.Id,
                    Time = t.Time,
                    TerminalId = t.TerminalId,
                    UserId = t.UserId,
                })
                .ToListAsync(ct);

            var count = await context.Terminals.AsNoTracking()
                .Where(u => !u.IsDeleted)
                .CountAsync(ct);

            return new PagedResult<EntryDto>
            {
                Items = entries,
                Take = take,
                Skip = skip,
                Total = count
            };
        }
    }
}