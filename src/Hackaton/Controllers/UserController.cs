using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResultSharp.HttpResult;
using Web.Extensions;

namespace API.Controllers
{
    [Route("api/[controller]")]
    public class UserController(
        IUserService userService) : ControllerBase
    {
        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> Info(Guid id, CancellationToken ct)
        {
            var result = await userService.GetInfo(id, ct);
            return result.ToResponse();
        }

        /// <summary>
        /// Требует авторизации
        /// </summary>
        [Authorize]
        [HttpGet("[action]")]
        public async Task<IActionResult> Info(CancellationToken ct)
        {
            var result = await userService.GetInfo(User.GetUserId(), ct);
            return result.ToResponse();
        }

        /// <summary>
        /// Требует авторизации
        /// </summary>
        [Authorize]
        [HttpGet("[action]")]
        public async Task<IActionResult> UpdateActivity()
        {
            var result = await userService.UpdateActivity(User.GetSessionId());
            return result.ToResponse();
        }
    }
}
