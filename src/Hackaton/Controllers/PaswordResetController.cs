using API.Contracts.Requests;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;
using ResultSharp.Core;
using ResultSharp.HttpResult;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    public class PaswordResetController(
        IResetPasswordService resetPasswordService) : ControllerBase
    {
        /// <summary>
        /// Отправляет на почту ссылку на фронт для сброса пароля
        /// </summary>
        [HttpGet("[action]/{email}")]
        public async Task<IActionResult> SendLink(string email, CancellationToken ct)
        {
            var result = await resetPasswordService.SendLink(email, ct);
            return result.ToResponse();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Validate([FromBody] ResetPasswordRequest request,
            CancellationToken ct)
        {
            var result = await resetPasswordService.ResetPassword(request.EmailId,
                   request.Password, ct);
            return result.ToResponse();
        }
    }
}
