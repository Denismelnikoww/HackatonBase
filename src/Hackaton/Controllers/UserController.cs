using Application.DTO;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Extensions;

namespace API.Controllers;

[Route("api/[controller]")]
public class UserController(
    IUserService userService) : ControllerBase
{
    [HttpGet("[action]/{id}")]
    [Produces(typeof(UserInfo))]
    public async Task<IActionResult> Info(Guid id, CancellationToken ct)
        => Ok(await userService.GetInfo(id, ct));

    /// <summary>
    /// Требует авторизации
    /// </summary>
    [Authorize]
    [HttpGet("[action]")]
    [Produces(typeof(UserInfo))]
    [ProducesResponseType(401)]
    public async Task<IActionResult> Info(CancellationToken ct)
        => Ok(await userService.GetInfo(User.GetUserId(), ct));
}