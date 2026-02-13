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

        // ---------- Toggle Group Message Star ----------
        [HttpPost("group/{groupMessageId}")]
        public async Task<IActionResult> ToggleGroupStar(Guid groupMessageId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? User.FindFirst("sub")?.Value;
            await _service.ToggleGroupStarAsync(Guid.Parse(userId), groupMessageId);
            return Ok();
        }


        // ---------- Get All Starred Messages ----------
        [HttpGet("all")]
        public async Task<IActionResult> GetAllStarredMessages()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? User.FindFirst("sub")?.Value;

            var starredMessages = await _service.GetAllStarredMessagesAsync(Guid.Parse(userId));
            return Ok(starredMessages);
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
