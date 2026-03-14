using API.Contracts.Requests;
using Application.Interfaces;
using Infrastructure.Interfaces;
using Infrastructure.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Web.Extensions;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly JwtOptions _options;
        private readonly IJwtProvider _jwtProvider;

        public AuthController(IAuthService authService,
            IOptions<JwtOptions> options,
            IJwtProvider jwtProvider)
        {
            _jwtProvider = jwtProvider;
            _authService = authService;
            _options = options.Value;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
        {
            var tokens = await _authService.Login(request.Login,
                request.Password,
                request.Email,
                ct);

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
            if (sessionIdClaim != null)
            {
                await _authService.Logout(Guid.Parse(sessionIdClaim), ct);
            }

            Response.Cookies.Delete(_options.AccessCookieName);
            Response.Cookies.Delete(_options.RefreshCookieName);

            return Ok();
        }


        [HttpPost("[action]")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
        {
            var result = await _authService.Register(request.Name,
                request.Login,
                request.Password,
                request.Email,
                ct);
            if (result) return Ok();
            return BadRequest(result);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Refresh(CancellationToken ct)
        {
            var refreshToken = Request.Cookies[_options.RefreshCookieName];

            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized();

            var principal = _jwtProvider.ValidateRefreshToken(refreshToken);
            if (principal == null)
                return Unauthorized();

            var userId = Guid.Parse(principal.GetUserId());
            var sessionId = Guid.Parse(principal.GetSessionId());

            var tokens = await _authService.Refresh(refreshToken, userId, sessionId, ct);

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