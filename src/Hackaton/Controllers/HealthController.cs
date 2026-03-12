using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private ILoadService _loadService;
        public HealthController(ILoadService loadService)
        {
            _loadService = loadService;
        }

        [HttpGet("[action]")]
        public async  Task<IActionResult> Live()
        {
            return Ok(DateTime.UtcNow);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> CurrentLoad()
        {
            return Ok(await _loadService.CurrentLoad());
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> SnapshotLoad()
        {
            return Ok(await _loadService.SnapshotLoad());
        }
    }
}
