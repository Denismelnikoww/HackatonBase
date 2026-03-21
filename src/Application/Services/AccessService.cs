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

        var user = await context.Users
            .Include(u => u.TerminalsAccess)
            .ThenInclude(uta => uta.Terminal)
            .FirstOrDefaultAsync(u => u.Id == userId, ct);

        if (user == null) return AccessStatus.Unknown;

        if (user.IsDeleted || user.IsBanned) return AccessStatus.Banned;

        if (!user.EntryAccess) return AccessStatus.Denied;

        var hasTerminalAccess = user.TerminalsAccess
            .Any(uta => uta.TerminalId == terminalId);

        if (!hasTerminalAccess)
            return AccessStatus.Denied;

        var terminal = await context.Terminals
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == terminalId && !t.IsDeleted, ct);

        if (terminal == null)
            return AccessStatus.Denied;

        var entry = new Entry
        {
            UserId = userId,
            TerminalId = terminalId,
        };

        await context.Entries.AddAsync(entry, ct);
        await context.SaveChangesAsync(ct);

        return AccessStatus.Granted;
    }
}