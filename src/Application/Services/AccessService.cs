using Application.Interfaces;
using Domain.Models;
using Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class AccessService(AppDbContext context, IQrService qrService) : IAccessService
{
    public async Task<AccessStatus> CheckAcess(Guid qrId, Guid terminalId, CancellationToken ct = default)
    {
        var userId = await qrService.Parse(qrId, ct);

        var user = await context.Users.AsNoTracking()
            .Where(u => u.Id == userId)
            .Select(u => new
            {
                IsDeleted = u.IsDeleted,
                IsBanned = u.IsBanned,
                Terminals = u.TerminalsAccess,
            })
            .FirstOrDefaultAsync(ct);

        if (user == null) return AccessStatus.Unknown;

        if (user.IsDeleted || user.IsBanned) return AccessStatus.Banned;

        if (!user.Terminals.Any(t => t.Id == terminalId)) return AccessStatus.Denied;

        await context.Entries.AddAsync(new Entry
        {
            UserId = userId,
            TerminalId = terminalId,
        }, ct);
        await context.SaveChangesAsync(ct);

        return AccessStatus.Granted;
    }
}