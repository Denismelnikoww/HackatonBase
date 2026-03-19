using Domain.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [Authorize(Roles = nameof(Role.Admin))]
    [Route("[controller]")]
    public class ApiKeyController : ControllerBase
    {
        [HttpGet("[action]")]
        public IActionResult Generate()
        {
            return Ok();
        }

        [HttpGet("[action]")]
        public IActionResult Revoke()
        {
            return Ok();
        }
    }
}
