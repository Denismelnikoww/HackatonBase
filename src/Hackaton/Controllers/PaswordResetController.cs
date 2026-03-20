using API.Contracts.Requests;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
#if RELEASE
    [ApiExplorerSettings(IgnoreApi = true)] 
#endif
    
    [Route("api/[controller]")]
    public class PaswordResetController(
        IResetPasswordByEmailService resetPasswordService) : ControllerBase
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
        public async Task<IActionResult> Validate([FromBody] ResetPasswordRequest request,
            CancellationToken ct)
        {
            await resetPasswordService.ResetPassword(request.EmailId,
                   request.Password, ct);
           return Ok();
        }
    }
}
