using Application.DTO;
using Application.Interfaces;
using Domain.Models.User;
using Infrastructure.DbContexts;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using ResultSharp.Core;
using ResultSharp.Errors;
using ResultSharp.Errors.Enums;

namespace Application.Services
{
    public class AuthService(
        UserDbContext context,
        IJwtProvider jwtProvider,
        IPasswordHasher passwordHasher) : IAuthService
    {
        public async Task<Result> Register(
            string name,
            string login,
            string password,
            string? email = null,
            CancellationToken ct = default)
        {
            email = email?.ToLower() ?? string.Empty;

            var user = await context.Users.AsNoTracking()
                .FirstOrDefaultAsync(u => (u.Login == login.ToLower())
                    || u.Email == email, ct);

            if (user != null)
                return Error.Conflict("Пользователь с таким логином и/или почтой уже существует");

            var newUser = new User
            {
                Name = name,
                Login = login,
                PasswordHash = passwordHasher.Hash(password),
            };
            newUser.ChangeEmail(email);

            await context.Users.AddAsync(newUser, ct);
            await context.SaveChangesAsync(ct);
            return Result.Success();
        }

        public async Task<Result<JwtTokens>> Login(string? login,
            string password,
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(login))
                return Error.BadRequest("Логин не должен быть пустым");

            login = login.ToLower() ?? string.Empty;

            var user = await context.Users.FirstOrDefaultAsync(u => (u.Login == login.ToLower())
                || (u.IsEmailConfirmed && u.Email == login), ct);

            if (user == null)
                return Error.NotFound("Такого пользователя не существует");

            if (!passwordHasher.Verify(password, user.PasswordHash))
                return Error.BadRequest("Неверный пароль");

            var session = new Session
            {
                UserId = user.Id,
            };

            var token = new Token
            {
                SessionId = session.Id,
            };

            var tokens = new JwtTokens
            {
                AccessToken = jwtProvider.GenerateAccessToken(user.Id, session.Id, user.Role),
                RefreshToken = jwtProvider.GenerateRefreshToken(user.Id, session.Id)
            };

            token.RefreshToken = tokens.RefreshToken;

            session.Token = token;

            await context.UserSessions.AddAsync(session, ct);
            await context.UserTokens.AddAsync(token, ct);

            await context.SaveChangesAsync(ct);

            return tokens;
        }

        public async Task<Result> Logout(Guid sessionId,
            CancellationToken ct = default)
        {
            var session = await context.UserSessions.FirstOrDefaultAsync(s => s.Id == sessionId, ct);

            if (session == null)
                return Error.NotFound("Такой сесиии не существует");

            session.IsActive = false;
            session.LastActivity = DateTime.UtcNow;
            session.LogoutDate = DateTime.UtcNow;
            await context.SaveChangesAsync(ct);

            return Result.Success();
        }

        public async Task<Result> ResetPassword(Guid userId,string oldPassword,string newPassword,CancellationToken ct)
        {
            var user = await context.Users.AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId, ct);

            if (user == null)
                return Error.NotFound("Такого пользователя не существует");

            if (user.IsDeleted || user.IsBanned)
                return Error.BadRequest("Пользователь удален или заблокирован");
            
            if (!passwordHasher.Verify(oldPassword, user.PasswordHash))
                return Error.BadRequest("Неверный пароль");

            user.PasswordHash = passwordHasher.Hash(newPassword);
            user.PasswordChangeDate = DateTime.Now;
            
            return Result.Success();
        }

        public async Task<Result<JwtTokens>> Refresh(string refreshToken,
            Guid userId,
            Guid sessionId,
            CancellationToken ct)
        {
            var user = await context.Users.AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId, ct);

            if (user == null)
                return Error.NotFound("Такого пользователя не существует");

            if (user.IsDeleted || user.IsBanned)
                return Error.BadRequest("Пользователь удален или заблокирован");

            var session = await context.UserSessions
                .Include(s => s.Token)
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == sessionId, ct);

            if (session == null)
                return Error.NotFound("Такой сессии не существует");

            if (session.Token?.RefreshToken != refreshToken)
            {
                session.IsActive = false;
                await context.SaveChangesAsync(ct);
                return new Error("Обнаружена подмена токена :)", ErrorCode.ImATeapot);
            }

            if (session.User.IsBanned)
                return Error.BadRequest("Пользователь заблокирован");

            if (!session.IsActive)
                return Error.BadRequest("Данная сессия не активна");

            session.Token.IsRevoked = true;

            var newToken = new Token
            {
                SessionId = sessionId,
                RefreshToken = jwtProvider.GenerateRefreshToken(userId, sessionId)
            };

            var tokens = new JwtTokens
            {
                AccessToken = jwtProvider.GenerateAccessToken(userId, sessionId, user.Role),
                RefreshToken = newToken.RefreshToken
            };

            session.TokenId = newToken.Id;
            session.LastActivity = DateTime.UtcNow;

            await context.UserTokens.AddAsync(newToken, ct);
            await context.SaveChangesAsync(ct);
            return tokens;
        }
    }
}
