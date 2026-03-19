using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResultSharp.HttpResult;
using Web.Extensions;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    public class EmailConfirmController(IEmailConfirmService emailConfirmService) : ControllerBase
    {
        /// <summary>
        /// Отправляет на почту ссылку на фронт для подтверждения почты
        /// </summary>
        [HttpGet("[action]/{email}")]
        public async Task<IActionResult> SendLink(string email, CancellationToken ct)
        {
            await emailConfirmService.SendLink(email, ct);
            return Ok();
        }

        /// <summary>
        /// Требует авторизации.
        /// Отправляет на почту ссылку на фронт для подтверждения почты
        /// </summary>
        [Authorize]
        [HttpGet("[action]")]
        [ProducesResponseType(401)]
        public async Task<IActionResult> SendLink(CancellationToken ct)
        {
            await emailConfirmService.SendLink(User.GetUserId(), ct);
            return Ok();
        }

        /// <summary>
        /// Подтверждение почты
        /// </summary>
        [HttpGet("{emailId}")]
        public async Task<IActionResult> Validate(Guid emailId, CancellationToken ct)
        {
            await emailConfirmService.ConfirmEmail(emailId.ToString(), ct);
            return Ok();
        }
    }
}
