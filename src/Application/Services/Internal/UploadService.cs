using Infrastructure.DbContexts;
using Domain.Models;
using Application.DTO;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Internal
{
    public class UploadService(AppDbContext context) : IUploadService
    {
        public async Task UploadUsersAsync(List<UserLargeDto> users, CancellationToken ct)
        {
            if (!users.Any()) return;

            var userIds = users.Select(u => u.Id).ToList();
            var userEmails = users.Where(u => !string.IsNullOrEmpty(u.Email)).Select(u => u.Email).ToList();

            var existingUsersById = await context.Users
                .Where(u => userIds.Contains(u.Id))
                .AsNoTracking()
                .ToDictionaryAsync(u => u.Id, ct);

            var existingUsersByEmail = await context.Users
                .Where(u => userEmails.Contains(u.Email))
                .AsNoTracking()
                .ToDictionaryAsync(u => u.Email, ct);

            var entitiesToInsert = new List<User>();
            var entitiesToUpdate = new List<User>();

            foreach (var userDto in users)
            {
                if (!string.IsNullOrEmpty(userDto.Email) && existingUsersByEmail.ContainsKey(userDto.Email))
                    continue;

                if (existingUsersById.TryGetValue(userDto.Id, out var existingUser))
                {
                    existingUser.Name = userDto.Name;
                    existingUser.Login = userDto.Login;
                    existingUser.ChangeEmail(userDto.Email);
                    existingUser.Role = userDto.Role;
                    existingUser.IsBanned = userDto.IsBanned;
                    existingUser.EntryAccess = userDto.EntryAccess;
                    entitiesToUpdate.Add(existingUser);
                }
                else
                {
                    var newUser = new User
                    {
                        Id = userDto.Id,
                        Name = userDto.Name,
                        Login = userDto.Login,
                        Role = userDto.Role,
                        IsBanned = userDto.IsBanned,
                        EntryAccess = userDto.EntryAccess
                    };
                    newUser.ChangeEmail(userDto.Email);
                    entitiesToInsert.Add(newUser);
                }
            }

            if (entitiesToInsert.Any())
                context.Users.AddRange(entitiesToInsert);

            await context.SaveChangesAsync(ct);
        }

        public async Task UploadTerminalsAsync(List<TerminalDto> terminals, CancellationToken ct)
        {
            if (!terminals.Any()) return;

            var terminalIds = terminals.Select(t => t.Id).ToList();

            var existingTerminals = await context.Terminals
                .Where(t => terminalIds.Contains(t.Id))
                .AsNoTracking()
                .ToDictionaryAsync(t => t.Id, ct);

            var entitiesToInsert = new List<Terminal>();
            var entitiesToUpdate = new List<Terminal>();

            foreach (var terminalDto in terminals)
            {
                if (existingTerminals.TryGetValue(terminalDto.Id, out var existingTerminal))
                {
                    existingTerminal.Name = terminalDto.Name;
                    existingTerminal.IsDeleted = false;
                    entitiesToUpdate.Add(existingTerminal);
                }
                else
                {
                    var newTerminal = new Terminal
                    {
                        Id = terminalDto.Id,
                        Name = terminalDto.Name,
                        IsDeleted = false
                    };
                    entitiesToInsert.Add(newTerminal);
                }
            }

            if (entitiesToInsert.Any())
                context.Terminals.AddRange(entitiesToInsert);

            await context.SaveChangesAsync(ct);
        }
    }
}