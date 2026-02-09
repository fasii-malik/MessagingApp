using MessagingApp.Server.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MessagingApp.Server.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class StarredMessagesController : ControllerBase
    {
        private readonly IStarredMessageService _service;

        public StarredMessagesController(IStarredMessageService service)
        {
            _service = service;
        }

        [HttpPost("{messageId}")]
        public async Task<IActionResult> ToggleStar(Guid messageId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? User.FindFirst("sub")?.Value;
            await _service.ToggleStarAsync(Guid.Parse(userId), messageId);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetStarredMessages()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? User.FindFirst("sub")?.Value;
            return Ok(await _service.GetStarredMessagesAsync(Guid.Parse(userId)));
        }
    }

}
