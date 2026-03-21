using Application.DTO;
using Application.Interfaces;
using Domain.Exceptions;
using Domain.Models;
using Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class ModeratorService(AppDbContext context) : IModeratorService
{
    public async Task<UserWithTerminalsDto> GetUser(Guid userId, CancellationToken ct = default)
    {
        var user = await context.Users.AsNoTracking()
            .Where(x => x.Id == userId && !x.EntryAccess && x.Role == Role.User)
            .Select(x => new UserWithTerminalsDto
            {
                Id = x.Id,
                EntryAccess = x.EntryAccess,
                Terminals = x.TerminalsAccess.Select(t => t.Id)
            })
            .FirstOrDefaultAsync(ct);

        if (user == null) throw new BadRequestException("Пользователя не существует");
        return user;
    }

    public async Task<PagedResult<UserSmallDto>> GetUsers(int take, int skip, CancellationToken ct = default)
    {
        var count = await context.Users.AsNoTracking()
            .Where(x => !x.IsDeleted && x.Role == Role.User)
            .CountAsync(ct);

        var users = await context.Users.AsNoTracking()
            .Where(x => !x.IsDeleted && x.Role == Role.User)
            .Skip(skip)
            .Take(take)
            .Select(x => new UserSmallDto
            {
                Id = x.Id,
                Email = x.Email,
                Name = x.Name,
                Role = x.Role,
            })
            .ToListAsync(ct);

        return new PagedResult<UserSmallDto>
        {
            Items = users,
            Take = take,
            Skip = skip,
            Total = count
        };
    }

    public async Task ChangeAccess(Guid userId, CancellationToken ct = default)
        => await context.Users
            .Where(x => x.Id == userId)
            .ExecuteUpdateAsync(setter => setter
                .SetProperty(u => u.EntryAccess, u => !u.EntryAccess), ct);

    public async Task SetTerminals(IEnumerable<Guid> terminalId, Guid userId, CancellationToken ct = default)
    {
        var user = await context.Users
            .Where(x => x.Id == userId)
            .FirstOrDefaultAsync(ct);

        if (user == null) throw new NotFoundException("Пользователя не существует");

        var terminals = await context.Terminals.AsNoTracking()
            .Where(x => terminalId.Contains(x.Id))
            .ToListAsync(ct);

        if (terminals.Count == 0) throw new NotFoundException("Ни один из выбранных терминалов не существует");

        user.TerminalsAccess = terminals;
    }

    public async Task<PagedResult<TerminalDto>> GetTerminals(Guid userId, int take, int skip,
        CancellationToken ct = default)
    {
        var userTerminalIds = await context.Users
            .Where(u => u.Id == userId)
            .SelectMany(u => u.TerminalsAccess.Select(t => t.Id))
            .ToListAsync(ct);

        var terminals = await context.Terminals.AsNoTracking()
            .OrderBy(t => t.Name)
            .Skip(skip)
            .Take(take)
            .Select(t => new TerminalDto
            {
                Id = t.Id,
                Name = t.Name,
                Access = userTerminalIds.Contains(t.Id) 
            })
            .ToListAsync(ct);

        var count = await context.Terminals.AsNoTracking().CountAsync(ct);

        return new PagedResult<TerminalDto>
        {
            Items = terminals,
            Skip = skip,
            Take = take,
            Total = count
        };
    }
}