using MessagingApp.Server.Application.Dtos;
using MessagingApp.Server.Application.Interfaces;
using MessagingApp.Server.Application.Services;
using MessagingApp.Server.Domain.Entities;
using MessagingApp.Server.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MessagingApp.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IPermissionService _permissionService;
        private readonly IUserService _userService;

        public UserController(IPermissionService permissionService, IUserService userService)
        {
            _permissionService = permissionService;
            _userService = userService;
        }
        
        [HttpGet("get-users")]
        public async Task<IActionResult> GetAllUsers()
        {            
            var users = await _userService.GetAllUsersAsync();
            
            var userDtos = users.Select(u => new UserDto
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                EmailConfirmed = u.EmailConfirmed,                
                Role = u.Role,
                CreatedAt = u.CreatedAt,                
                Permissions = u.Permissions.Select(p => p.Permission).ToList()
            }).ToList();

            return Ok(userDtos);
        }

        [Authorize(Policy = "AssignPermissions")]        
        [HttpPost("assign-permissions")]
        public async Task<IActionResult> AssignPermissions(AssignPermissionRequest request)
        {
            await _permissionService.AssignAsync(
                request.UserId,
                request.Permissions);

            return Ok();
        }
               
        [HttpPost("remove-permissions")]
        [Authorize(Policy = "AssignPermissions")]
        public async Task<IActionResult> RemovePermission(AssignPermissionRequest request)
        {
            await _permissionService.RevokeAsync(
                request.UserId,
                request.Permissions);

            return Ok();
        }

        [HttpPost("assign-role")]
        [Authorize(Policy = "AssignRoles")]
        public async Task<IActionResult> AssignRole(AssignRoleRequest request)
        {
            var currentUserRoleClaim = User.FindFirst(ClaimTypes.Role);

            if (currentUserRoleClaim == null)
                return Unauthorized();

            var currentUserRole = Enum.Parse<UserRole>(currentUserRoleClaim.Value);

            await _userService.AssignRoleAsync(
                request.UserId,
                request.Role,
                currentUserRole
            );

            return Ok("Role assigned successfully");
        }

        //[Authorize]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetCurrentUser(Guid id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            var dto = new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email
            };

            return Ok(dto);
        }

    }
}
