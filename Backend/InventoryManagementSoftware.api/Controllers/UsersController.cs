using InventoryManagementSoftware.api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace InventoryManagementSoftware.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _service;

        public UsersController(IUserService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var user = await _service.GetUserByIdAsync(id);
                if (user == null)
                    return NotFound(new { message = "User not found" });

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to retrieve user", details = ex.Message });
            }
        }

        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetByEmail(string email)
        {
            try
            {
                var user = await _service.GetUserByEmailAsync(email);
                if (user == null)
                    return NotFound(new { message = "User not found" });

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to retrieve user", details = ex.Message });
            }
        }

        [HttpGet("username/{username}")]
        public async Task<IActionResult> GetByUsername(string username)
        {
            try
            {
                var user = await _service.GetUserByUsernameAsync(username);
                if (user == null)
                    return NotFound(new { message = "User not found" });

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to retrieve user", details = ex.Message });
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var users = await _service.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to retrieve users", details = ex.Message });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.Email) || 
                    string.IsNullOrWhiteSpace(request.Username) || 
                    string.IsNullOrWhiteSpace(request.Password))
                    return BadRequest(new { message = "Email, username, and password are required" });

                var user = await _service.RegisterAsync(request.Email, request.Username, request.Password);
                return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to register user", details = ex.Message });
            }
        }

        [HttpPost("{id}/block")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> BlockUser(string id)
        {
            try
            {
                var success = await _service.BlockUserAsync(id);
                if (!success)
                    return NotFound(new { message = "User not found" });

                return Ok(new { message = "User blocked successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to block user", details = ex.Message });
            }
        }

        [HttpPost("{id}/unblock")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UnblockUser(string id)
        {
            try
            {
                var success = await _service.UnblockUserAsync(id);
                if (!success)
                    return NotFound(new { message = "User not found" });

                return Ok(new { message = "User unblocked successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to unblock user", details = ex.Message });
            }
        }

        [HttpPost("{id}/make-admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> MakeAdmin(string id)
        {
            try
            {
                var success = await _service.MakeAdminAsync(id);
                if (!success)
                    return NotFound(new { message = "User not found" });

                return Ok(new { message = "User is now an admin" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to make admin", details = ex.Message });
            }
        }

        [HttpPost("{id}/remove-admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveAdmin(string id)
        {
            try
            {
                var success = await _service.RemoveAdminAsync(id);
                if (!success)
                    return NotFound(new { message = "User not found" });

                return Ok(new { message = "Admin role removed" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to remove admin", details = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                var success = await _service.DeleteUserAsync(id);
                if (!success)
                    return NotFound(new { message = "User not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to delete user", details = ex.Message });
            }
        }

        [HttpPost("self-demote")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SelfDemoteAdmin()
        {
            try
            {
                var userId = User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { message = "User ID not found" });

                var success = await _service.SelfDemoteAdminAsync(userId);
                if (!success)
                    return BadRequest(new { message = "Failed to demote admin role" });

                return Ok(new { message = "Admin role removed successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to self-demote", details = ex.Message });
            }
        }
    }

    public class RegisterRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
