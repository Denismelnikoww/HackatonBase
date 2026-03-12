using Domain.Models.User;
using Infrastructure.Interfaces;
using Infrastructure.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Auth
{
    public class JwtProvider : IJwtProvider
    {
        private readonly JwtSettings _settings;
        public JwtProvider(IOptions<JwtSettings> jwtSettings)
        {
            _settings = jwtSettings.Value;
        }

        public string GenerateAccessToken(Guid userId, Guid sessionId, Role role)
        {
            Claim[] claims = [new Claim(_settings.UserIdCookieName, userId.ToString()),
                new Claim(_settings.SessionCookieName, sessionId.ToString()),
                new Claim(ClaimTypes.Role, role.ToString())];

            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret)),
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                expires: DateTime.Now.AddMinutes(_settings.AccessTokenExpirationMinutes),
                signingCredentials: signingCredentials);

            var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

            return tokenValue;
        }

        public string GenerateRefreshToken(Guid userId, Guid sessionId)
        {
            Claim[] claims = [new Claim(_settings.UserIdCookieName, userId.ToString()),
                new Claim(_settings.SessionCookieName,sessionId.ToString())];

            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret)),
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                expires: DateTime.Now.AddDays(_settings.RefreshTokenExpirationDays),
                signingCredentials: signingCredentials);

            var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

            return tokenValue;
        }

        public ClaimsPrincipal? ValidateRefreshToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_settings.Secret);

            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _settings.Issuer,
                ValidateAudience = true,
                ValidAudience = _settings.Audience,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero
            }, out _);

            if (!principal.HasClaim(c => c.Type == _settings.UserIdCookieName) ||
                !principal.HasClaim(c => c.Type == _settings.SessionCookieName))
                return null;

            return principal;
        }
    }
}
