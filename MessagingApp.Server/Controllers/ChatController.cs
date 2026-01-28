using MessagingApp.Server.Application.Dtos;
using MessagingApp.Server.Application.Interfaces;
using MessagingApp.Server.Domain.Entities;
using MessagingApp.Server.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace MessagingApp.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly IUserConnectionService _connections;

        public ChatController(IMessageService messageService, IUserConnectionService connections)
        {
            _messageService = messageService;
            _connections = connections;
        }

        [Authorize]
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetChat(string userId)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized();

            var messages = await _messageService.GetChatAsync(currentUserId, userId);
            return Ok(messages);
        }

        [Authorize]
        [HttpGet("online-users")]
        public async Task<IActionResult> GetOnlineUsers()
        {
            // Get current user ID from JWT
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized();

            // Fetch all users except current from DB
            var users = await _messageService.GetAllUsersAsync(currentUserId);
            

            return Ok(users);
        }

        // GET: api/chat/conversations
        [HttpGet("conversations")]
        public async Task<IActionResult> GetConversations()
        {
            try
            {
                // 1️⃣ Get current user id from JWT (assuming you have a method for that)
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? User.FindFirst("sub")?.Value; // "sub" claim contains userId

                if (string.IsNullOrEmpty(currentUserId))
                    return Unauthorized("User not authenticated.");

                // 2️⃣ Get conversations from service
                var conversations = await _messageService.GetConversationsAsync(currentUserId);

                // 3️⃣ Return
                return Ok(conversations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("delete-messages")]
        public async Task<IActionResult> DeleteMessages(
            [FromBody] DeleteMessagesRequest request)
        {
            if (request.MessageIds == null || !request.MessageIds.Any())
                return BadRequest("No message IDs provided.");

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            var currentUserId = Guid.Parse(userIdClaim);

            await _messageService.DeleteMessagesAsync(
                request.MessageIds,
                currentUserId);

            return NoContent(); // ✅ standard REST response
        }

        [HttpDelete("delete-conversation/{otherUserId}")]
        public async Task<IActionResult> DeleteConversation(Guid otherUserId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? User.FindFirst("sub")?.Value; // Current user Id from JWt

            if (userId == null)
                return Unauthorized();

            await _messageService.DeleteConversationAsync(Guid.Parse(userId), otherUserId);

            return Ok(new { Message = "Conversation deleted successfully" });
        }


    }

}
