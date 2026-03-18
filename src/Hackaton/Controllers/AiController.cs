using Infrastructure.Interfaces;
using Infrastructure.Mistral;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResultSharp.Core;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    public class AiController(IMistralService mistralService) : ControllerBase
    {
        /// <summary>
        /// Пока чисто для меня. Потом воткнем куда-то
        /// </summary>
        [HttpPost("[action]")]
        public async Task<MistralResponse> Send(string message, CancellationToken ct)
            => await mistralService.CreateConversationAsync(message);

    }
}
