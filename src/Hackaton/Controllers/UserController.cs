using Application.DTO;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResultSharp.Core;
using ResultSharp.HttpResult;
using Web.Extensions;

namespace API.Controllers
{
    [Route("api/[controller]")]
    public class UserController(
        IUserService userService) : ControllerBase
    {
        [HttpGet("[action]/{id}")]
        [Produces(typeof(Result<UserInfo?>))]
        public async Task<IActionResult> Info(Guid id, CancellationToken ct)
            => await userService.GetInfo(id, ct).ToResponseAsync();

        /// <summary>
        /// Требует авторизации
        /// </summary>
        [Authorize]
        [HttpGet("[action]")]
        [Produces(typeof(Result<UserInfo?>))]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Info(CancellationToken ct)
            => await userService.GetInfo(User.GetUserId(), ct).ToResponseAsync();

        /// <summary>
        /// Требует авторизации
        /// </summary>
        [Authorize]
        [HttpGet("[action]")]
        [Produces(typeof(Result))]
        [ProducesResponseType(401)]
        public async Task<IActionResult> UpdateActivity()
            => await userService.UpdateActivity(User.GetSessionId()).ToResponseAsync();
    }
}
