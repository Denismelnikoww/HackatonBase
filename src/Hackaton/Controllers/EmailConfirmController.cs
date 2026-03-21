using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Extensions;

namespace Web.Controllers;

[Route("api/[controller]")]
public class EmailConfirmController(IEmailConfirmService emailConfirmService) : ControllerBase
{
    /// <summary>
    /// Требует авторизации.
    /// Отправляет на почту ссылку на фронт для подтверждения почты
    /// </summary>
    [Authorize]
    [HttpGet("[action]")]
    [ProducesResponseType(401)]
    public async Task<IActionResult> SendLink(CancellationToken ct)
    {
        await emailConfirmService.SendLink(User.GetUserId(), ct);
        return Ok();
    }

    /// <summary>
    /// Подтверждение почты
    /// </summary>
    [HttpGet()]
    public async Task<IActionResult> Validate([FromQuery] Guid emailId, CancellationToken ct)
    {
        await emailConfirmService.ConfirmEmail(emailId.ToString(), ct);
        return Ok();
    }
}