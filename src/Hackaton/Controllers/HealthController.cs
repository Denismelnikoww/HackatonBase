using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController(ILoadService loadService) : ControllerBase
    {
        [HttpGet("[action]")]
        public async Task<IActionResult> Live()
        {
            return Ok(DateTime.UtcNow);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> CurrentLoad()
        {
            return Ok(await loadService.CurrentLoad());
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> SnapshotLoad()
        {
            return Ok(await loadService.SnapshotLoad());
        }
    }
}
