using API.Contracts.Requests;
using Infrastructure.Interfaces;
using Infrastructure.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Web.Extensions;

namespace Web.Controllers;

[Authorize]
[ProducesResponseType(401)]
[Route("api/password")]
public class PasswordResetController(
    IResetPasswordService resetPasswordService,
    IOptions<JwtOptions> options,
    IJwtProvider jwtProvider) : ControllerBase
{

    /// <summary>
    /// Смена пароля из профиля
    /// </summary>
    [HttpPost("[action]")]
    public async Task<IActionResult> Change([FromBody] ResetPasswordRequest request,
        CancellationToken ct)
    {
        await resetPasswordService.ResetPassword(User.GetUserId(), request.OldPassword,
            request.NewPassword, ct);
        return Ok("Пароль успешно сменен");
    }

    /// <summary>
    /// Смена пароля по почте
    /// </summary>
    [HttpPost("[action]")]
    public async Task<IActionResult> Reset([FromQuery] string password, CancellationToken ct)
    {
        var refreshToken = Request.Cookies[options.Value.ResetPasswordCookieName];

        if (string.IsNullOrEmpty(refreshToken)) return Unauthorized("Как ты сюда попал?");

        var principal = jwtProvider.ValidatePasswordResetToken(refreshToken);
        if (principal == null) return Unauthorized("Токен не корректен");

        var email = principal.GetEmail();
        await resetPasswordService.ResetPassword(email, password, ct);

        return Ok();
    }
}