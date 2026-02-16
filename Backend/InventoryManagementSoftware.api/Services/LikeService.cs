using InventoryManagementSoftware.api.Dtos;
using InventoryManagementSoftware.api.Data;
using InventoryManagementSoftware.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSoftware.api.Services
{
    public class LikeService : ILikeService
    {
        private readonly AppDbContext _context;

        public LikeService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LikeDto>> GetLikesByItemAsync(Guid itemId)
        {
            var likes = await _context.Likes
                .Where(l => l.ItemId == itemId)
                .Include(l => l.User)
                .ToListAsync();

            return likes.Select(l => new LikeDto
            {
                ItemId = l.ItemId,
                UserId = l.UserId,
                UserName = l.User?.UserName ?? "Unknown",
                CreatedAt = l.CreatedAt
            });
        }

        public async Task<bool> IsLikedByUserAsync(Guid itemId, string userId)
        {
            return await _context.Likes
                .AnyAsync(l => l.ItemId == itemId && l.UserId == userId);
        }

        public async Task<ToggleLikeResponse> ToggleLikeAsync(Guid itemId, string userId, string userName)
        {
            var item = await _context.Items.FindAsync(itemId);
            if (item == null)
                throw new Exception("Item not found");

            var existingLike = await _context.Likes
                .FirstOrDefaultAsync(l => l.ItemId == itemId && l.UserId == userId);

            if (existingLike != null)
            {
                _context.Likes.Remove(existingLike);
                item.LikeCount = Math.Max(0, item.LikeCount - 1);
            }
            else
            {
                var newLike = new Like
                {
                    ItemId = itemId,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                };
                await _context.Likes.AddAsync(newLike);
                item.LikeCount++;
            }

            await _context.SaveChangesAsync();

            return new ToggleLikeResponse
            {
                IsLiked = existingLike == null,
                TotalLikes = item.LikeCount
            };
        }

        public async Task<int> GetLikeCountAsync(Guid itemId)
        {
            return await _context.Likes
                .CountAsync(l => l.ItemId == itemId);
        }
    }
}
