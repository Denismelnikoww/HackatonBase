using Application.Interfaces;
using Infrastructure.DbContexts;
using Infrastructure.Interfaces;
using Infrastructure.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ResultSharp.Core;
using ResultSharp.Errors;
using ResultSharp.Errors.Enums;

namespace Application.Services
{
    public class EmailConfirmService(
        IRedisCacheService redisCacheService,
        IOptions<VerificationCacheOptions> options,
        IEmailTemplateBuilder emailTemplateBuilder,
        IPasswordHasher passwordHasher,
        IEmailService emailService,
        UserDbContext context) : IEmailConfirmService
    {
        private readonly VerificationCacheOptions _options = options.Value;

        public async Task<Result> SendLink(string email, CancellationToken ct)
        {
            var user = await context.Users.AsNoTracking()
                .Where(u => u.Email == email)
                .Select(u => new
                {
                    IsEmailConfirmed = u.IsEmailConfirmed
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return Error.NotFound("Такого пользователя не существует");
            if (user.IsEmailConfirmed)
                return Error.BadRequest("Почта данного пользователя уже подтверждена");

            var emailId = Guid.NewGuid().ToString();
            var link = $"https://{emailId}";

            await redisCacheService.SetAsync(emailId, email,
                TimeSpan.FromMinutes(_options.EmailExpirationMinutes));

            var html = emailTemplateBuilder.BuildEmailConfirmation(link, _options.EmailExpirationMinutes);

            await emailService.SendAsync(email, "Подтверждение почты", html, ct);

            return Result.Success();
        }

        public async Task<Result> SendLink(Guid userId, CancellationToken ct)
        {
            var user = await context.Users.AsNoTracking()
                .Where(u => u.Id == userId)
                .Select(u => new
                {
                    Email = u.Email,
                    IsEmailConfirmed = u.IsEmailConfirmed
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return Error.NotFound("Такого пользователя не существует");
            if (string.IsNullOrWhiteSpace(user.Email))
                return Error.BadRequest("У пользователя не указана почта");
            if (user.IsEmailConfirmed)
                return Error.BadRequest("Почта данного пользователя уже подтверждена");

            var emailId = Guid.NewGuid().ToString();
            var link = $"https://{emailId}";

            await redisCacheService.SetAsync(emailId, user.Email,
                TimeSpan.FromMinutes(_options.EmailExpirationMinutes));

            var html = emailTemplateBuilder.BuildEmailConfirmation(link, _options.EmailExpirationMinutes);

            await emailService.SendAsync(user.Email, "Подтверждение почты", html, ct);

            return Result.Success();
        }

        public async Task<Result> ConfirmEmail(string emailId, CancellationToken ct)
        {
            var email = await redisCacheService.GetAsync<string>(emailId);
            if (email == null)
                return new Error("Даже не представляю зачем и как ты сюда попал :)", ErrorCode.ImATeapot);

            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                return Error.NotFound("Такого пользователя не существует");

            user.ConfirmEmail();

            await context.SaveChangesAsync();

            return Result.Success();
        }
    }
}

