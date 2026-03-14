//using API.Contracts.Requests;
//using Infrastructure.Interfaces;
//using Microsoft.AspNetCore.Mvc;

//namespace Web.Controllers
//{
//    [Route("api/[controller]")]
//    public class PaswordResetController : ControllerBase
//    {
//        private readonly IResetPasswordService _resetPasswordService;


//        /// <summary>
//        /// Отправляет на почту ссылку на фронт для сброса пароля
//        /// </summary>
//        [HttpGet("[action]/{email}")]
//        public async Task<IActionResult> SendLink(string email, CancellationToken ct)
//        {
//            await _resetPasswordService.SendLink(email, ct);
//            return Ok();
//        }

//        [HttpPost("[action]")]
//        public async Task<IActionResult> Validate([FromBody] ResetPasswordRequest request,
//            CancellationToken ct)
//        {
//            await _resetPasswordService.ResetPassword(request.Email,
//                request.Token,
//                request.Password,
//                ct);
//            return Ok();
//        }
//    }
//}
