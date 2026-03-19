using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [ApiController]
    [Route("internal/[controller]")]
    public class UploadController : ControllerBase
    {
        [HttpPost("[action]")]
        public async Task<IActionResult> Users()
        {
            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Terminals()
        {
            return Ok();
        }
    }
}
