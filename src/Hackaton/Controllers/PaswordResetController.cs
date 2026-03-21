using API.Contracts.Requests;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Extensions;

namespace Web.Controllers;

[Authorize]
[ProducesResponseType(401)]
[Route("api/[controller]")]
public class PaswordResetController(
    IResetPasswordService resetPasswordService) : ControllerBase
{
    [HttpPost("[action]")]
    public async Task<IActionResult> Validate([FromBody] ResetPasswordRequest request,
        CancellationToken ct)
    {
        await resetPasswordService.ResetPassword(User.GetUserId(), request.OldPassword, request.NewPassword, ct);
        return Ok("Пароль успешно сменен");
    }
}