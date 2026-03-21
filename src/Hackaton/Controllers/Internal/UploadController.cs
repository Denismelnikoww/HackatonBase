using Microsoft.AspNetCore.Mvc;
using Application.Interfaces;
using Application.DTO;

namespace Web.Controllers.Internal;

[ApiController]
[Route("internal/[controller]")]
public class UploadController(IUploadService uploadService) : ControllerBase
{
    [HttpPost("[action]")]
    public async Task<IActionResult> Users([FromBody] List<UserLargeDto> users, CancellationToken ct)
    {
        await uploadService.UploadUsersAsync(users, ct);
        return Ok();
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Terminals([FromBody] List<TerminalDto> terminals, CancellationToken ct)
    {
        await uploadService.UploadTerminalsAsync(terminals, ct);
        return Ok();
    }
}