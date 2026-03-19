using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet("[action]")]
        public async Task<DateTime> Live()
           => DateTime.UtcNow;
    }
}
