using API.Contracts.Requests;
using Application.Interfaces;
using Infrastructure.Interfaces;
using Infrastructure.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private IAuthService _authService;
        private JwtSettings _jwtSettings;
        private IJwtProvider _jwtProvider;

        public AuthController(IAuthService authService,
            IOptions<JwtSettings> jwtSettings,
            IJwtProvider jwtProvider)
        {
            _jwtProvider = jwtProvider;
            _authService = authService;
            _jwtSettings = jwtSettings.Value;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
        {
            var tokens = await _authService.Login(request.Login,
                request.Password,
                request.Email,
                request.Phone,
                ct);

            Response.Cookies.Append(_jwtSettings.AccessCookieName, tokens.AccessToken, new CookieOptions
            {
                Secure = true,
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
                MaxAge = TimeSpan.FromMinutes(_jwtSettings.AccessTokenExpirationMinutes)
            });
            Response.Cookies.Append(_jwtSettings.RefreshCookieName, tokens.RefreshToken, new CookieOptions
            {
                Secure = true,
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
                MaxAge = TimeSpan.FromDays(_jwtSettings.RefreshTokenExpirationDays)
            });

            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Logout(CancellationToken ct)
        {
            Response.Cookies.Delete(_jwtSettings.AccessCookieName);
            Response.Cookies.Delete(_jwtSettings.RefreshCookieName);

            var sessionIdClaim = User.FindFirst(_jwtSettings.SessionCookieName)?.Value;
            if (sessionIdClaim != null)
            {
                await _authService.Logout(Guid.Parse(sessionIdClaim), ct);
            }

            return Ok();
        }


        [HttpPost("[action]")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
        {
            var result = await _authService.Register(request.Login,
                request.Password,
                request.Email,
                request.Phone,
                ct);
            if (result) return Ok();
            return BadRequest(result);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Refresh(CancellationToken ct)
        {
            var refreshToken = Request.Cookies[_jwtSettings.RefreshCookieName];

            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized();

            var principal = _jwtProvider.ValidateRefreshToken(refreshToken);
            if (principal == null)
                return Unauthorized();

            var userId = Guid.Parse(principal.FindFirst(_jwtSettings.UserIdCookieName)?.Value);
            var sessionId = Guid.Parse(principal.FindFirst(_jwtSettings.SessionCookieName)?.Value);

            var tokens = await _authService.Refresh(refreshToken, userId, sessionId, ct);
            Response.Cookies.Append(_jwtSettings.AccessCookieName, tokens.AccessToken, new CookieOptions
            {
                IsEssential = true,
                Secure = true,
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
                MaxAge = TimeSpan.FromMinutes(_jwtSettings.AccessTokenExpirationMinutes)
            });
            Response.Cookies.Append(_jwtSettings.RefreshCookieName, tokens.RefreshToken, new CookieOptions
            {
                IsEssential = true,
                Secure = true,
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
                MaxAge = TimeSpan.FromDays(_jwtSettings.RefreshTokenExpirationDays)
            });

            return Ok();
        }

        [HttpGet("[action]/{email}")]
        public async Task<IActionResult> ForgotPassword(string email, CancellationToken ct)
        {
            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ResetPassword(string email, string token, CancellationToken ct)
        {
            return Ok();
        }
    }
}