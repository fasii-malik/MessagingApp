using MessagingApp.Server.Application.Dtos;
using MessagingApp.Server.Application.Interfaces;
using MessagingApp.Server.Application.Repositories;
using MessagingApp.Server.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MessagingApp.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]    
    [IgnoreAntiforgeryToken]
    public class GroupChatController : ControllerBase
    {
        private readonly IGroupService _groupService;
        private readonly IGroupMessageService _groupMessageService;
        private readonly IUserService _userService;

        public GroupChatController(IGroupService groupService, IGroupMessageService groupMessageService)
        {
            _groupService = groupService;
            _groupMessageService = groupMessageService;
        }

        [Authorize]
        [HttpPost("create-groups")]
        public async Task<IActionResult> CreateGroup(CreateGroupDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? User.FindFirst("sub")?.Value;

            var group = await _groupService.CreateGroupAsync(
                dto.Name,
                Guid.Parse(userId),
                dto.MemberIds
            );

            return Ok(group);
        }

        [Authorize]
        [HttpGet("get-groups")]
        public async Task<IActionResult> GetMyGroups()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? User.FindFirst("sub")?.Value;

            return Ok(await _groupService.GetUserGroupsAsync(Guid.Parse(userId)));
        }

        [HttpGet("{groupId}/messages")]
        public async Task<ActionResult<List<GroupMessageDto>>> GetGroupMessages(Guid groupId)
        {
            var messages = await _groupMessageService.GetGroupMessagesAsync(groupId);
            return Ok(messages);
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteGroupMessages([FromBody] List<Guid> ids)
        {
            await _groupMessageService.DeleteGroupMessagesAsync(ids);
            return Ok();
        }

        [HttpPost("{groupId}/leave")]
        public async Task<IActionResult> LeaveGroup(Guid groupId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // from JWT

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var success = await _groupService.LeaveGroupAsync(groupId, Guid.Parse(userId));

            if (!success)
                return BadRequest("Failed to leave the group.");

            return Ok("Left group successfully.");
        }

        [HttpGet("{groupId}/info")]
        public async Task<IActionResult> GetGroupInfo(Guid groupId)
        {
            if (groupId == Guid.Empty)
                return BadRequest("Invalid group ID.");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // from JWT

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var groupInfo = await _groupService.GetGroupInfoAsync(groupId, Guid.Parse(userId));

            if (groupInfo == null)
                return NotFound("Group not found.");

            return Ok(groupInfo);
        }

        [Authorize]
        [HttpPost("{groupId}/add-member")]
        public async Task<IActionResult> AddMember(
    Guid groupId,
    [FromBody] Guid userId)
        {
            var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier); // from JWT

            var success = await _groupService.AddMemberAsync(groupId, Guid.Parse(adminId), userId);

            if (!success)
                return BadRequest("Unable to add member");

            return Ok();
        }

    }
}
