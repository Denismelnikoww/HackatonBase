using Application.DTO;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Web.Contracts.Requests;

namespace Web.Controllers.Internal
{
    /// <summary>
    /// Контроллер для экспорта данных (пользователи, терминалы, события входа) во внешние системы
    /// </summary>
    [ApiController]
    [Route("internal/[controller]")]
    public class ImportController(IImportService importService) : ControllerBase
    {
        /// <summary>
        /// Получить список пользователей с пагинацией
        /// </summary>
        [HttpGet("[action]")]
        [Produces(typeof(PagedResult<UserLargeDto>))]
        public async Task<IActionResult> Users([FromQuery] PagedRequest request, CancellationToken ct)
            => Ok(await importService.GetUsers(request.Take, request.Skip, ct));


        /// <summary>
        /// Получить список терминалов с пагинацией
        /// </summary>
        [HttpGet("[action]")]
        [Produces(typeof(PagedResult<TerminalDto>))]
        public async Task<IActionResult> Terminals([FromQuery] PagedRequest request, CancellationToken ct)
            => Ok(await importService.GetTerminals(request.Take, request.Skip, ct));

        /// <summary>
        /// Получить журнал входов с пагинацией
        /// </summary>
        [HttpGet("[action]")]
        [Produces(typeof(PagedResult<EntryDto>))]
        public async Task<IActionResult> Entry([FromQuery] PagedRequest request, CancellationToken ct)
            => Ok(await importService.GetEntries(request.Take, request.Skip, ct));
    }
}
