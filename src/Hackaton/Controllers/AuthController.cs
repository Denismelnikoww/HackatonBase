using API.Contracts.Requests;
using Application.Interfaces;
using Infrastructure.Interfaces;
using Infrastructure.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ResultSharp.Core;
using ResultSharp.Errors;
using ResultSharp.HttpResult;
using Web.Extensions;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces(typeof(Result))]
    public class AuthController(IAuthService authService,
        IOptions<JwtOptions> options,
        IJwtProvider jwtProvider) : ControllerBase
    {
        private readonly JwtOptions _options = options.Value;

        [HttpPost("[action]")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
        {
            var result = await authService.Login(request.Login,
                request.Password, ct);

            if (result.IsFailure)
                return result.ToResponse();

            var tokens = result.Value;
            SetTokens(tokens.AccessToken, tokens.RefreshToken);

            return Result.Success().ToResponse();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Logout(CancellationToken ct)
        {
            var sessionIdClaim = User.FindFirst(_options.SessionCookieName)?.Value;
            if (sessionIdClaim == null)
                return Result.Failure(Error.BadRequest("Сессия не активна")).ToResponse();

            var result = await authService.Logout(Guid.Parse(sessionIdClaim), ct);

            DeleteTokens();

            return result.ToResponse();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
        {
            return await authService.Register(request.Name,
                request.Login,
                request.Password,
                request.Email,
                ct)
                .ToResponseAsync();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Refresh(CancellationToken ct)
        {
            var refreshToken = Request.Cookies[_options.RefreshCookieName];

            if (string.IsNullOrEmpty(refreshToken))
                return Result.Failure(Error.Unauthorized("Отсутсвует рефреш токен")).ToResponse();

            var principal = jwtProvider.ValidateRefreshToken(refreshToken);
            if (principal == null)
                return Result.Failure(Error.Unauthorized("Рефреш токен не корректен")).ToResponse();

            var userId = principal.GetUserId();
            var sessionId = principal.GetSessionId();

            var result = await authService.Refresh(refreshToken, userId, sessionId, ct);

            if (result.IsFailure)
                return result.ToResponse();

            var tokens = result.Value;
            SetTokens(tokens.AccessToken, tokens.RefreshToken);

            return result.ToResponse();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request, CancellationToken ct )
        {
            
            return Ok();
        }

        private void SetTokens(string accessToken, string refreshToken)
        {
            Response.Cookies.Append(_options.AccessCookieName, accessToken, new CookieOptions
            {
                IsEssential = true,
                Secure = true,
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
                MaxAge = TimeSpan.FromMinutes(_options.AccessTokenExpirationMinutes)
            });
            Response.Cookies.Append(_options.RefreshCookieName, refreshToken, new CookieOptions
            {
                IsEssential = true,
                Secure = true,
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
                MaxAge = TimeSpan.FromDays(_options.RefreshTokenExpirationDays)
            });
        }

        private void DeleteTokens()
        {
            Response.Cookies.Delete(_options.AccessCookieName);
            Response.Cookies.Delete(_options.RefreshCookieName);
        }
    }
}