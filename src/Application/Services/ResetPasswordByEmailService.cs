using Domain.Exceptions;
using Infrastructure.DbContexts;
using Infrastructure.Interfaces;
using Infrastructure.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Application.Services
{
    public class ResetPasswordByEmailService(
        IOptions<VerificationOptions> options,
        IEmailTemplateBuilder emailTemplateBuilder,
        IPasswordHasher passwordHasher,
        IEmailService emailService,
        AppDbContext context) : IResetPasswordByEmailService
    {
        private readonly VerificationOptions _options = options.Value;

        public async Task SendPassword(string email, CancellationToken ct)
        {
            var user = await context.Users.AsNoTracking()
                .Where(u => u.Email == email)
                .Select(u => new
                {
                    IsEmailConfirmed = u.IsEmailConfirmed
                })
                .FirstOrDefaultAsync();

            if (user == null) throw new NotFoundException("Такого пользователя не существует");
            if (!user.IsEmailConfirmed) throw new BadRequestException("Почта данного пользователя не подтверждена");

            var emailId = Guid.NewGuid().ToString();
            var link = $"https://{emailId}";

            var html = emailTemplateBuilder.BuildResetPasswordEmail(link, _options.EmailExpirationMinutes);

            await emailService.SendAsync(email, "Смена пароля", html, ct);
        }
    }
}