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
    public class ResetPasswordService(IRedisCacheService redisCacheService,
        IOptions<VerificationCacheOptions> options,
        IEmailTemplateBuilder emailTemplateBuilder,
        IPasswordHasher passwordHasher,
        IEmailService emailService,
        UserDbContext context) : IResetPasswordService
    {
        private readonly VerificationCacheOptions _options = options.Value;

        public async Task<Result> SendLink(string email, CancellationToken ct)
        {
            // TODO: на проде изменить что почта подтверждена
            var user = await context.Users.AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
                return Error.NotFound("Такого пользователя не существует");

            var emailId = Guid.NewGuid().ToString();
            var link = $"https://{emailId}";

            await redisCacheService.SetAsync(emailId, email,
                TimeSpan.FromMinutes(_options.EmailExpirationMinutes));

            var html = emailTemplateBuilder.BuildResetPasswordEmail(link, _options.EmailExpirationMinutes);

            await emailService.SendAsync(email, "Смена пароля", html, ct);

            return Result.Success();
        }

        public async Task<Result> ResetPassword(string emailId, string password, CancellationToken ct)
        {
            var email = await redisCacheService.GetAsync<string>(emailId);

            if (email == null)
                return new Error("А вот и не получилось поменять чужой пароль :)", ErrorCode.ImATeapot);

            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
                return Error.NotFound("Такого пользователя не существует");

            user.PasswordHash = passwordHasher.Hash(password);
            if (!user.IsEmailConfirmed)
                user.ConfirmEmail();

            await context.SaveChangesAsync();

            return Result.Success();
        }
    }
}