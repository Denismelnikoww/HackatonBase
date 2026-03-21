using API.Contracts.Requests;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[Route("api/[controller]")]
public class PaswordResetByEmailController(
    IResetPasswordByEmailService resetPasswordByEmailService) : ControllerBase
{
    /// <summary>
    /// Отправляет на почту ссылку на фронт для сброса пароля
    /// </summary>
    [HttpGet("[action]")]
    public async Task<IActionResult> SendLink([FromQuery] string email, CancellationToken ct)
    {
        await resetPasswordByEmailService.SendLink(email, ct);
        return Ok();
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Validate([FromBody] ResetPasswordByEmailRequest byEmailRequest,
        CancellationToken ct)
    {
        await resetPasswordByEmailService.ResetPassword(byEmailRequest.EmailId,
            byEmailRequest.Password, ct);
        return Ok();
    }
}