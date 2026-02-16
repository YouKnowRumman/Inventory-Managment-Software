using InventoryManagementSoftware.api.Data;
using InventoryManagementSoftware.api.Dtos;
using InventoryManagementSoftware.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSoftware.api.Services
{
    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public bool IsBlocked { get; set; }
        public bool IsAdmin { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Language { get; set; }
        public string? Theme { get; set; }
    }

    public interface IUserService
    {
        Task<UserDto?> GetUserByIdAsync(string id);
        Task<UserDto?> GetUserByEmailAsync(string email);
        Task<UserDto?> GetUserByUsernameAsync(string username);
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto> RegisterAsync(string email, string username, string password);
        Task<bool> BlockUserAsync(string userId);
        Task<bool> UnblockUserAsync(string userId);
        Task<bool> MakeAdminAsync(string userId);
        Task<bool> RemoveAdminAsync(string userId);
        Task<bool> DeleteUserAsync(string userId);
        Task<bool> UpdateUserAsync(string userId, UserDto dto);
        Task<bool> SelfDemoteAdminAsync(string userId);
    }

    public class UserService : IUserService
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(AppDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<UserDto?> GetUserByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            return user == null ? null : MapToDto(user);
        }

        public async Task<UserDto?> GetUserByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user == null ? null : MapToDto(user);
        }

        public async Task<UserDto?> GetUserByUsernameAsync(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            return user == null ? null : MapToDto(user);
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            return users.Select(MapToDto);
        }

        public async Task<UserDto> RegisterAsync(string email, string username, string password)
        {
            var user = new ApplicationUser
            {
                Email = email,
                UserName = username,
                EmailConfirmed = true // Skip email confirmation for MVP
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                throw new InvalidOperationException($"User creation failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");

            return MapToDto(user);
        }

        public async Task<bool> BlockUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            user.IsBlocked = true;
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> UnblockUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            user.IsBlocked = false;
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> MakeAdminAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            var result = await _userManager.AddToRoleAsync(user, "Admin");
            return result.Succeeded;
        }

        public async Task<bool> RemoveAdminAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            var result = await _userManager.RemoveFromRoleAsync(user, "Admin");
            return result.Succeeded;
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> UpdateUserAsync(string userId, UserDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            user.UserName = dto.UserName ?? user.UserName;
            user.Email = dto.Email ?? user.Email;
            user.Language = dto.Language ?? user.Language;
            user.Theme = dto.Theme ?? user.Theme;

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> SelfDemoteAdminAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            var result = await _userManager.RemoveFromRoleAsync(user, "Admin");
            return result.Succeeded;
        }

        private static UserDto MapToDto(ApplicationUser user)
        {
            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                FullName = user.UserName,
                IsBlocked = user.IsBlocked,
                IsAdmin = user.Id != null, // Placeholder - should check roles
                CreatedAt = DateTime.UtcNow,
                Language = user.Language,
                Theme = user.Theme
            };
        }
    }
}
