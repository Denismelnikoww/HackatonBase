using Domain.Models.User;
using Infrastructure.DbContexts;
using Infrastructure.Interfaces;
using Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace Infrastructure.Services
{
    public class ResetPasswordTokensProvider : IResetPasswordTokensProvider
    {

        private readonly UserDbContext _context;
        private readonly DbSet<PasswordResetToken> _passwordTokens;
        private readonly ResetPasswordTokenSettings _settings;

        public ResetPasswordTokensProvider(UserDbContext userDbContext,
            IOptions<ResetPasswordTokenSettings> settings)
        {
            _context = userDbContext;
            _passwordTokens = userDbContext.Set<PasswordResetToken>();
            _settings = settings.Value;
        }

        public async Task<int> GetPasswordResetToken(string email)
        {
            var token = await _passwordTokens.FirstOrDefaultAsync(t => !t.IsUsed
                && t.Email == email
                && t.ExpiresAt > DateTime.UtcNow);

            if (token == null)
            {
                token.ExpiresAt = DateTime.UtcNow.AddMinutes(_settings.TokenExpirationMinutes);
                token.Token = RandomNumberGenerator.GetInt32(100000, 1000000);

                await _context.SaveChangesAsync();

                return token.Token;
            }

            var newToken = new PasswordResetToken
            {
                Email = email,
            };

            await _passwordTokens.AddAsync(newToken);
            await _context.SaveChangesAsync();
            return newToken.Token;
        }

        public async Task<bool> VerifyPasswordResetToken(string email, int inputToken)
        {
            var token = await _passwordTokens.FirstOrDefaultAsync(t => !t.IsUsed
                            && t.Email == email
                            && t.ExpiresAt > DateTime.UtcNow);

            if (token == null)
                return false;

            return token.Token == inputToken;
        }
    }
}