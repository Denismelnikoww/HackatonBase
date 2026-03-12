using Application.DTO;
using Application.Interfaces;
using Domain.Models.User;
using Infrastructure.DbContexts;
using Infrastructure.Extensions;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserDbContext _context;
        private readonly DbSet<User> _users;
        private readonly DbSet<Session> _sessions;
        private readonly DbSet<Token> _tokens;

        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtProvider _jwtProvider;

        public AuthService(UserDbContext context, IJwtProvider jwtProvider, IPasswordHasher passwordHasher)
        {
            _context = context;
            _users = _context.Set<User>();
            _sessions = _context.Set<Session>();
            _tokens = _context.Set<Token>();

            _passwordHasher = passwordHasher;
            _jwtProvider = jwtProvider;
        }

        public async Task<bool> Register(
            string login,
            string password,
            string? email = null,
            string? phone = null,
            CancellationToken ct = default)
        {
            email = email?.ToLower() ?? string.Empty;
            phone ??= string.Empty;

            var user = await _users.AsNoTracking()
                .FirstOrDefaultAsync(u => (u.Login == login.ToLower()) ||
                u.Email == email || u.Phone == phone, ct);

            if (user != null)
                return false;

            var newUser = new User
            {
                Login = login,
                PasswordHash = _passwordHasher.Hash(password),
            };
            newUser.ChangePhone(phone);
            newUser.ChangeEmail(email);

            await _users.AddAsync(newUser, ct);
            await _context.SaveChangesAsync(ct);
            return true;
        }

        public async Task<JwtTokens> Login(string? login,
            string password,
            string? email = null,
            string? phone = null,
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(login)
                && string.IsNullOrWhiteSpace(email)
                && string.IsNullOrWhiteSpace(phone))
                throw new ArgumentException("At least one identifier (login, email, or phone) must be provided");

            login ??= string.Empty;
            email ??= string.Empty;
            phone ??= string.Empty;

            var user = await _users.FirstOrDefaultAsync(u => (u.Login == login.ToLower()) ||
                (u.IsEmailConfirmed && u.Email == email.ToLower()) ||
                (u.IsPhoneConfirmed && u.Phone == phone), ct);

            if (user == null)
                throw new Exception("User not found");

            if (!_passwordHasher.Verify(password, user.PasswordHash))
                throw new Exception("Invalid password");

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
                AccessToken = _jwtProvider.GenerateAccessToken(user.Id, session.Id, user.Role),
                RefreshToken = _jwtProvider.GenerateRefreshToken(user.Id, session.Id)
            };

            token.RefreshToken = tokens.RefreshToken;

            session.Token = token;

            await _sessions.AddAsync(session, ct);
            await _tokens.AddAsync(token, ct);

            await _context.SaveChangesAsync(ct);

            return tokens;
        }

        public async Task Logout(Guid sessionId,
            CancellationToken ct = default)
        {
            var session = await _sessions.FirstOrDefaultAsync(s => s.Id == sessionId, ct);

            if (session == null)
                return;

            session.IsActive = false;
            session.LastActivity = DateTime.UtcNow;
            session.LogoutDate = DateTime.UtcNow;
            await _context.SaveChangesAsync(ct);
        }

        public async Task<JwtTokens> Refresh(string refreshToken,
            Guid userId,
            Guid sessionId,
            CancellationToken ct)
        {
            var user = await _users.AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId, ct);

            if (user == null || user.IsDeleted || user.IsBanned)
                throw new Exception();

            var session = await _sessions
                .Include(s => s.Token)
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == sessionId, ct);

            if (session == null)
                throw new SecurityException("Session not found");

            if (session.Token?.RefreshToken != refreshToken)
            {
                session.IsActive = false;
                await _context.SaveChangesAsync(ct);
                throw new SecurityException("Token reuse detected");
            }

            if (session.UserId != userId || session.User.IsBanned || !session.IsActive)
                throw new SecurityException("Invalid session");

            session.Token.IsRevoked = true;

            var newToken = new Token
            {
                SessionId = sessionId,
                RefreshToken = _jwtProvider.GenerateRefreshToken(userId, sessionId)
            };

            var tokens = new JwtTokens
            {
                AccessToken = _jwtProvider.GenerateAccessToken(userId, sessionId, user.Role),
                RefreshToken = newToken.RefreshToken
            };

            session.TokenId = newToken.Id;
            session.LastActivity = DateTime.UtcNow;

            await _tokens.AddAsync(newToken, ct);
            await _context.SaveChangesAsync(ct);
            return tokens;
        }
    }
}
