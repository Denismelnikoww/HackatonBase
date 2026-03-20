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
        [Authorize]
        [Produces(typeof(Guid))]
        [HttpGet("[action]")]
        public IActionResult Generate(CancellationToken ct)
            => Ok(qrService.Generate(User.GetUserId()));
    }
}