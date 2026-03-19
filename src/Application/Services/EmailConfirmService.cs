using Application.Interfaces;
using Domain.Exceptions;
using Infrastructure.DbContexts;
using Infrastructure.Interfaces;
using Infrastructure.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ResultSharp.Errors;

namespace Application.Services
{
    public class EmailConfirmService(
        IRedisCacheService redisCacheService,
        IOptions<VerificationOptions> options,
        IEmailTemplateBuilder emailTemplateBuilder,
        IPasswordHasher passwordHasher,
        IEmailService emailService,
        UserDbContext context) : IEmailConfirmService
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
            if (user.IsEmailConfirmed) throw new ConflictException("Почта данного пользователя уже подтверждена");

            var emailId = Guid.NewGuid().ToString();
            var link = $"https://{_options.ConfirmEmailLink}?emailId={emailId}";

            await redisCacheService.SetAsync(emailId, email,
                TimeSpan.FromMinutes(_options.EmailExpirationMinutes));

            var html = emailTemplateBuilder.BuildEmailConfirmation(link, _options.EmailExpirationMinutes);

            await emailService.SendAsync(email, "Подтверждение почты", html, ct);
        }

        public async Task SendLink(Guid userId, CancellationToken ct)
        {
            var user = await context.Users.AsNoTracking()
                .Where(u => u.Id == userId)
                .Select(u => new
                {
                    Email = u.Email,
                    IsEmailConfirmed = u.IsEmailConfirmed
                })
                .FirstOrDefaultAsync();

            if (user == null) throw new NotFoundException("Такого пользователя не существует");
            if (string.IsNullOrWhiteSpace(user.Email)) throw new BadRequestException("Укажите почту прежде чем ее подтверждать");
            if (user.IsEmailConfirmed) throw new ConflictException("Почта данного пользователя уже подтверждена");

            var emailId = Guid.NewGuid().ToString();
            var link = $"https://{emailId}";

            await redisCacheService.SetAsync(emailId, user.Email,
                TimeSpan.FromMinutes(_options.EmailExpirationMinutes));

            var html = emailTemplateBuilder.BuildEmailConfirmation(link, _options.EmailExpirationMinutes);

            await emailService.SendAsync(user.Email, "Подтверждение почты", html, ct);
        }

        public async Task ConfirmEmail(string emailId, CancellationToken ct)
        {
            var email = await redisCacheService.GetAsync<string>(emailId);

            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null) throw new NotFoundException("Такого пользователя не существует");

            user.ConfirmEmail();

            await context.SaveChangesAsync();
        }
    }
}

