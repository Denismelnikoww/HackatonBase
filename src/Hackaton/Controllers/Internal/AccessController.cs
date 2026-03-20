using Application.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.Internal
{
    namespace Web.Controllers
    {
        [Route("internal/[controller]")]
        public class AccessController(IAccessService accessService) : ControllerBase
        {
            /// <summary>
            /// Проверяет доступ пользователя к данному терминалу
            /// Возвращает:
            /// 0 - разрешить
            /// 1 - запретить.  у пользователя нет доступа к данному терминалу
            /// 2 - истек срок токена или неизвестные причины
            /// 3 - пользователь был заблокирован или удален
            /// </summary>
            /// <param name="qrId">GUID который считал терминал</param>
            /// <param name="terminalId">GUID терминала</param>
           
            [HttpGet]
            [Produces(typeof(AccessStatus))]
            public async Task<IActionResult> Access([FromQuery] Guid qrId, Guid terminalId, CancellationToken ct)
                => Ok(await accessService.CheckAcess(qrId, terminalId, ct));

            /// <summary>
            /// Проверяет доступ пользователя к данному терминалу.
            /// Возвращает текстовый статус доступа
            /// </summary>
            /// <param name="qrId">GUID который считал терминал</param>
            /// <param name="terminalId">GUID терминала</param>
            [HttpGet("[action]")]
            [Produces(typeof(AccessStatus))]
            public async Task<IActionResult> Text([FromQuery] Guid qrId, Guid terminalId, CancellationToken ct)
                => Ok((await accessService.CheckAcess(qrId, terminalId, ct)).GetDescription());
        }
    }
}