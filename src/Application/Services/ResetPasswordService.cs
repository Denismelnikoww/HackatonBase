using Domain.Exceptions;
using Infrastructure.DbContexts;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class ResetPasswordService(
    IPasswordHasher passwordHasher,
    AppDbContext context) : IResetPasswordService
{
    public async Task ResetPassword(Guid userId, string oldPassword, string newPassword, CancellationToken ct)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId, ct);
        if (user == null) throw new NotFoundException("Такого пользователя не существует");
        if (!passwordHasher.Verify(oldPassword, user.PasswordHash))
            throw new BadRequestException("Страрый пароль не совпадает с введенным");

        user.PasswordHash = passwordHasher.Hash(newPassword);

        await context.SaveChangesAsync();
    }
}