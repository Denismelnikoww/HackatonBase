using Infrastructure.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Web.Extensions;

namespace Web.Controllers
{
    [Authorize]
    [Route("/")]
    public class EmailConfirmController : ControllerBase
    {

        public EmailConfirmController(IOptions<JwtSettings> settings)
        {

        }

        [HttpGet]
        public async Task<IActionResult> SendLink()
        {
            var userId = User.GetUserId();

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Validate()
        {

        }
    }
}
