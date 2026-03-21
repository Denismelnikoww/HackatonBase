using Application.DTO;
using Application.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Contracts.Requests;

namespace Web.Controllers;

[ApiController]
[Authorize(Roles = $"{nameof(Role.Moderator)},{nameof(Role.Admin)}")]
[ProducesResponseType(401)]
[Route("api/moderate")]
public class ModeratorController(IModeratorService moderatorService) : ControllerBase
{
    [HttpGet("[action]")]
    [Produces(typeof(UserWithTerminalsDto))]
    public async Task<IActionResult> User([FromQuery] Guid userId, CancellationToken ct)
        => Ok(await moderatorService.GetUser(userId, ct));

    /// <summary>
    /// Получить всех ПОЛЬЗОВАТЕЛЕЙ. АДМИНЫ И МОДЕРАТОРЫ НЕ ПРИДУТ
    /// </summary>
    [HttpGet("[action]")]
    [Produces(typeof(PagedResult<UserSmallDto>))]
    public async Task<IActionResult> Users([FromQuery] PagedRequest request, CancellationToken ct)
        => Ok(await moderatorService.GetUsers(request.Take, request.Skip, ct));


    [HttpPatch("[action]")]
    public async Task<IActionResult> SetAccessSettings([FromBody] AccessSetingsRequest setingsRequest,
        CancellationToken ct)
    {
        await moderatorService.SetAccessSettings(setingsRequest.Terminals, setingsRequest.UserId,
            setingsRequest.EntryAccess, ct);
        return Ok();
    }

    [Produces(typeof(PagedResult<TerminalDto>))]
    [HttpGet("[action]")]
    public async Task<IActionResult> Terminals([FromQuery] PagedRequest request, [FromQuery] Guid userId,
        CancellationToken ct)
        => Ok(await moderatorService.GetTerminals(userId, request.Take, request.Skip, ct));
}