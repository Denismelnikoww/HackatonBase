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
    [HttpGet()]
    [ProducesResponseType(401)]
    public async Task<IActionResult> SendToken(CancellationToken ct)
    {
        await emailConfirmService.SendToken(User.GetUserId(), ct);
        return Ok();
    }

    /// <summary>
    /// Требует авторизации.
    /// Подтверждение почты
    /// </summary>
    [HttpGet("[action]")]
    [ProducesResponseType(401)]
    public async Task<IActionResult> Confirm([FromQuery] string token, CancellationToken ct)
    {
        await emailConfirmService.ConfirmEmail(User.GetUserId(), token, ct);
        return Ok();
    }
}