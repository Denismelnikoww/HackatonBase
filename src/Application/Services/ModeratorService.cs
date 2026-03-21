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
        var user = await context.Users
            .AsNoTracking()
            .Where(x => x.Id == userId && !x.IsDeleted && x.Role == Role.User)
            .Select(x => new UserWithTerminalsDto
            {
                Id = x.Id,
                EntryAccess = x.EntryAccess,
                Terminals = x.TerminalsAccess.Select(ta => ta.TerminalId) // Исправлено: берем TerminalId из связной таблицы
            })
            .FirstOrDefaultAsync(ct);

        if (user == null) 
            throw new BadRequestException("Пользователя не существует или он удален");
        
        return user;
    }

    public async Task<PagedResult<UserSmallDto>> GetUsers(int take, int skip, CancellationToken ct = default)
    {
        var count = await context.Users
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.Role == Role.User)
            .CountAsync(ct);

        var users = await context.Users
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.Role == Role.User)
            .OrderBy(x => x.Name) 
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

    public async Task SetAccessSettings(IEnumerable<Guid> terminalIds, Guid userId, bool entryAccess,
        CancellationToken ct = default)
    {
        var user = await context.Users
            .Include(x => x.TerminalsAccess)
            .FirstOrDefaultAsync(x => x.Id == userId, ct);

        if (user == null) 
            throw new NotFoundException("Пользователя не существует или он был забанен");

        if (user.IsDeleted)
            throw new BadRequestException("Пользователь удален");

        var terminalIdsList = terminalIds.Distinct().ToList();
        
        var existingTerminals = await context.Terminals
            .Where(x => terminalIdsList.Contains(x.Id) && !x.IsDeleted)
            .Select(x => x.Id)
            .ToListAsync(ct);

        if (existingTerminals.Count != terminalIdsList.Count)
            throw new NotFoundException("Некоторые терминалы не найдены или удалены");

        user.TerminalsAccess.Clear();
        
        foreach (var terminalId in existingTerminals)
        {
            user.TerminalsAccess.Add(new UserTerminalAccess
            {
                UserId = userId,
                TerminalId = terminalId
            });
        }
        
        user.EntryAccess = entryAccess;
        
        await context.SaveChangesAsync(ct);
    }

    public async Task<PagedResult<TerminalDto>> GetTerminals(Guid userId, int take, int skip,
        CancellationToken ct = default)
    {
        var userTerminalIds = await context.UserTerminalAccess
            .AsNoTracking()
            .Where(uta => uta.UserId == userId)
            .Select(uta => uta.TerminalId)
            .ToListAsync(ct);

        var totalCount = await context.Terminals
            .AsNoTracking()
            .Where(t => !t.IsDeleted)
            .CountAsync(ct);

        var terminals = await context.Terminals
            .AsNoTracking()
            .Where(t => !t.IsDeleted)
            .OrderBy(t => t.Name)
            .Skip(skip)
            .Take(take)
            .Select(t => new TerminalDto
            {
                Id = t.Id,
                Name = t.Name,
                EntryAccess = userTerminalIds.Contains(t.Id)
            })
            .ToListAsync(ct);

        return new PagedResult<TerminalDto>
        {
            Items = terminals,
            Skip = skip,
            Take = take,
            Total = totalCount
        };
    }
}