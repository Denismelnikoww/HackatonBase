using Domain.Exceptions;
using Infrastructure.DbContexts;
using Infrastructure.Interfaces;
using Infrastructure.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Application.Services
{
    public class ResetPasswordByEmailService(IRedisCacheService redisCacheService,
        IOptions<VerificationOptions> options,
        IEmailTemplateBuilder emailTemplateBuilder,
        IPasswordHasher passwordHasher,
        IEmailService emailService,
        AppDbContext context) : IResetPasswordByEmailService
    {
        private readonly VerificationOptions _options = options.Value;

        public async Task SendLink(string email, CancellationToken ct)
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

            await redisCacheService.SetAsync(emailId, email,
                TimeSpan.FromMinutes(_options.EmailExpirationMinutes));

            var html = emailTemplateBuilder.BuildResetPasswordEmail(link, _options.EmailExpirationMinutes);

            await emailService.SendAsync(email, "Смена пароля", html, ct);
        }

        public async Task ResetPassword(string emailId, string password, CancellationToken ct)
        {
            var email = await redisCacheService.GetAsync<string>(emailId);
            if (email == null) throw new TeapotException("А вот и не получилось поменять чужой пароль :)");

            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) throw new NotFoundException("Такого пользователя не существует");

            user.PasswordHash = passwordHasher.Hash(password);

            await context.SaveChangesAsync();
        }
    }
}