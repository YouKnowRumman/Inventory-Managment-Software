using InventoryManagementSoftware.api.Services;
using InventoryManagementSoftware.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace InventoryManagementSoftware.api.Controllers
{
    public class AuthLoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class AuthRegisterRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUserService _userService;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IUserService userService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AuthRegisterRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Username) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { message = "Email, username, and password are required" });
            }

            try
            {
                var user = await _userService.RegisterAsync(request.Email, request.Username, request.Password);
                await _signInManager.SignInAsync(new ApplicationUser { Id = user.Id, UserName = user.UserName }, false);
                return Ok(new { message = "Registration successful", user });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Registration failed", details = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthLoginRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { message = "Email and password are required" });
            }

            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null || user.IsBlocked)
                {
                    return Unauthorized(new { message = "Invalid credentials or account blocked" });
                }

                var result = await _signInManager.PasswordSignInAsync(user, request.Password, false, lockoutOnFailure: false);
                if (!result.Succeeded)
                {
                    return Unauthorized(new { message = "Invalid credentials" });
                }

                var userDto = new
                {
                    id = user.Id,
                    userName = user.UserName,
                    email = user.Email,
                    isAdmin = false // TODO: Check roles
                };

                return Ok(new { message = "Login successful", user = userDto });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Login failed", details = ex.Message });
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _signInManager.SignOutAsync();
                return Ok(new { message = "Logout successful" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Logout failed", details = ex.Message });
            }
        }

        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Unauthorized(new { message = "Not authenticated" });
                }

                return Ok(new
                {
                    id = user.Id,
                    userName = user.UserName,
                    email = user.Email,
                    isAdmin = false // TODO: Check roles
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to get current user", details = ex.Message });
            }
        }
    }
}
