using API.Contracts.Requests;
using Application.Interfaces;
using Infrastructure.Interfaces;
using Infrastructure.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ResultSharp.HttpResult;
using Web.Extensions;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IAuthService authService,
        IOptions<JwtOptions> options,
        IJwtProvider jwtProvider) : ControllerBase
    {
        private readonly JwtOptions _options = options.Value;

        [HttpPost("[action]")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
        {
            var result = await authService.Login(request.Login,
                request.Password,
                request.Email,
                ct);

            if (result.IsFailure)
                return result.ToResponse();

            var tokens = result.Value;

            Response.Cookies.Append(_options.AccessCookieName, tokens.AccessToken, new CookieOptions
            {
                Secure = true,
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
                MaxAge = TimeSpan.FromMinutes(_options.AccessTokenExpirationMinutes)
            });
            Response.Cookies.Append(_options.RefreshCookieName, tokens.RefreshToken, new CookieOptions
            {
                Secure = true,
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
                MaxAge = TimeSpan.FromDays(_options.RefreshTokenExpirationDays)
            });

            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Logout(CancellationToken ct)
        {
            var sessionIdClaim = User.FindFirst(_options.SessionCookieName)?.Value;
            if (sessionIdClaim == null)
                return BadRequest();

            var result = await authService.Logout(Guid.Parse(sessionIdClaim), ct);

            Response.Cookies.Delete(_options.AccessCookieName);
            Response.Cookies.Delete(_options.RefreshCookieName);

            return result.ToResponse();
        }


        [HttpPost("[action]")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
        {
            var result = await authService.Register(request.Name,
                request.Login,
                request.Password,
                request.Email,
                ct);

            return result.ToResponse();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Refresh(CancellationToken ct)
        {
            var refreshToken = Request.Cookies[_options.RefreshCookieName];

            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized();

            var principal = jwtProvider.ValidateRefreshToken(refreshToken);
            if (principal == null)
                return Unauthorized();

            var userId = principal.GetUserId();
            var sessionId = principal.GetSessionId();

            var result = await authService.Refresh(refreshToken, userId, sessionId, ct);
            if (result.IsFailure)
                return result.ToResponse();

            var tokens = result.Value;

            Response.Cookies.Append(_options.AccessCookieName, tokens.AccessToken, new CookieOptions
            {
                IsEssential = true,
                Secure = true,
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
                MaxAge = TimeSpan.FromMinutes(_options.AccessTokenExpirationMinutes)
            });
            Response.Cookies.Append(_options.RefreshCookieName, tokens.RefreshToken, new CookieOptions
            {
                IsEssential = true,
                Secure = true,
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
                MaxAge = TimeSpan.FromDays(_options.RefreshTokenExpirationDays)
            });

            return Ok();
        }
    }
}