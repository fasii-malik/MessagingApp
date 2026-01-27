using MessagingApp.Server.Application.Dtos;
using MessagingApp.Server.Application.Interfaces;
using MessagingApp.Server.Application.Security;
using MessagingApp.Server.Application.Services;
using MessagingApp.Server.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MessagingApp.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IUserService _userService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly AuthService _authService;

        public AuthController(IUserService userService, IPasswordHasher passwordHasher, AuthService authService)
        {
            _userService = userService;
            _passwordHasher = passwordHasher;
            _authService = authService;
        }

        [HttpGet("by-email")]
        public  async Task<IActionResult> GetByEmail(string email)
        {
            var user = await _userService.GetUserByEmail(email);

            if (user is null)
                return NotFound("User not found or inactive");

            var userDto = new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
                Permissions = user.Permissions.Select(p => p.Permission).ToList()
            };

            return Ok(userDto);            
        }

        [HttpGet("by-id")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await _userService.GetByIdAsync(id);

            if (user is null)
                return NotFound("User not found or inactive");
            
            var userDto = new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
                Permissions = user.Permissions.Select(p => p.Permission).ToList()                
            };

            return Ok(userDto);
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(CreateUserRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
           // string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                FullName = request.FullName,
                Email = request.Email,
                PasswordHash = _passwordHasher.Hash(request.Password),                                                                
            };


            try
            {                
                await _userService.AddAsync(user);
                return Ok("User registered successfully");
            }
            catch (ArgumentNullException)
            {
                return BadRequest("Invalid user data");
            }
            catch (Exception ex)
            {
                // log exception here
                return StatusCode(500, "An unexpected error occurred");
            }
        }

        [Authorize(Policy = "ManageUsers")]
        [HttpPut("edit/{id}")]        
        public async Task<IActionResult> UpdateUser(Guid id, UpdateUserRequest request)
        {            
            if (id == Guid.Empty)
                return BadRequest("Invalid user id");

            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var isSelf = currentUserId == id.ToString();
            var isAdmin = User.HasClaim("role", "Admin") || User.HasClaim("role", "SuperAdmin");

            if (!isSelf && !isAdmin)
                return Forbid(); ; 
            

            var user = new User
            {
                Id = id,
                FullName = request.FullName,
                Email = request.Email,                
            };

            try
            {
                await _userService.UpdateAsync(user);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [Authorize(Policy = "ManageUsers")]
        [HttpDelete("delete/{id}")]        
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("Invalid user id");

            try
            {
                await _userService.DeleteAsync(id);
                return NoContent(); // 204
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var response = await _authService.LoginAsync(request);

            if (response is null)
                return Unauthorized("Invalid email or password");

            //// ACCESS TOKEN in HttpOnly cookie
            //Response.Cookies.Append(
            //    "access_token",
            //    response.Token,
            //    new CookieOptions
            //    {
            //        HttpOnly = true,
            //        Secure = true,              // must be true in production
            //        SameSite = SameSiteMode.Lax,
            //        Expires = response.Expiration,
            //        IsEssential = true
            //    });

            //// Store refresh token too (recommended)
            //Response.Cookies.Append(
            //    "refresh_token",
            //    response.RefreshToken,
            //    new CookieOptions
            //    {
            //        HttpOnly = true,
            //        Secure = true,
            //        SameSite = SameSiteMode.Lax,
            //        Expires = DateTime.UtcNow.AddDays(7)
            //    });


            return Ok(new
            {
                token = response.Token,
            });
        }

        //[HttpPost("logout")]
        //[Authorize]
        //public async Task<IActionResult> Logout()
        //{
        //    var refreshToken = Request.Cookies["refresh_token"];

        //    if (string.IsNullOrWhiteSpace(refreshToken))
        //        return BadRequest();

        //    await _authService.LogoutAsync(refreshToken);

        //    Response.Cookies.Delete("access_token");
        //    Response.Cookies.Delete("refresh_token");

        //    return Ok();
        //}

        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh(RefreshTokenRequest request)
        {
            var result = await _authService.RefreshAsync(request.RefreshToken);

            if (result is null)
                return Unauthorized("Invalid refresh token");

            return Ok(result);
        }

    }//class
}//namespace
