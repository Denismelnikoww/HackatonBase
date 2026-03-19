using API.Contracts.Requests;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    public class PaswordResetByEmailController(
        IResetPasswordService resetPasswordService) : ControllerBase
    {
        /// <summary>
        /// Отправляет на почту ссылку на фронт для сброса пароля
        /// </summary>
        [HttpGet("[action]/{email}")]
        public async Task<IActionResult> SendLink(string email, CancellationToken ct)
        {
            await resetPasswordService.SendLink(email, ct);
            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Validate([FromBody] ResetPasswordByEmailRequest byEmailRequest,
            CancellationToken ct)
        {
            await resetPasswordService.ResetPassword(byEmailRequest.EmailId,
                   byEmailRequest.Password, ct);
            return Ok();
        }
    }
}