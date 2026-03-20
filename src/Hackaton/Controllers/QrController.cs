using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Extensions;

namespace Web.Controllers
{
#if RELEASE
    [ApiExplorerSettings(IgnoreApi = true)]
#endif

    [Route("api/[controller]")]
    public class QrController(IQrService qrService) : ControllerBase
    {
        /// <summary>
        /// Генерирует GUID который будет преобразован в QR.
        /// Требует авторизации
        /// </summary>
        [Authorize]
        [Produces(typeof(Guid))]
        [HttpGet("[action]")]
        public IActionResult Generate(CancellationToken ct)
            => Ok(qrService.Generate(User.GetUserId()));

        /// <summary>
        /// Отзывает GUID который был получен из api/qr/generate.
        /// Используется в случаях:
        /// - обновления пользователем QR
        /// - закрытия приложения
        /// - попытке сделать скриншот 
        /// </summary>
        /// <param name="qrId">Guid который был получен из api/qr/generate</param>
        [Produces(typeof(Guid))]
        [HttpGet("[action]")]
        public IActionResult Revoke([FromQuery] Guid qrId, CancellationToken ct)
            => Ok(qrService.Revoke(qrId, ct));
    }
}