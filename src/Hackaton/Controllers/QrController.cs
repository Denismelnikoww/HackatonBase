using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    public class QrController : ControllerBase
    {
        [Authorize]
        [HttpGet("[action]")]
        public IActionResult Generate()
        {
            return Ok();
        }
    }
}
