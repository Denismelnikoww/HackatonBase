using Application.DTO;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Extensions;

namespace API.Controllers
{
    [Route("api/[controller]")]
    public class UserController(
        IUserService userService) : ControllerBase
    {
        [HttpGet("[action]/{id}")]
        [Produces(typeof(UserInfo))]
        public async Task<IActionResult> Info(Guid id, CancellationToken ct)
        {
            await userService.GetInfo(id, ct);
            return Ok();
        }

        /// <summary>
        /// Требует авторизации
        /// </summary>
        [Authorize]
        [HttpGet("[action]")]
        [Produces(typeof(UserInfo))]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Info(CancellationToken ct)
        {
            await userService.GetInfo(User.GetUserId(), ct);
            return Ok();
        }
    }
}
