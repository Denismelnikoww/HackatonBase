using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet("[action]")]
    public async Task<DateTime> Live()
        => DateTime.UtcNow;
}