using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.Internal
{
    namespace Web.Controllers
    {
        [Route("internal/[controller]")]
        public class AcessController : ControllerBase
        {
            [Authorize]
            [HttpGet("[action]")]
            public IActionResult Validate()
            {
                return Ok();
            }
        }
    }
}
