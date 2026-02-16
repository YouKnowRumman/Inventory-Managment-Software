using InventoryManagementSoftware.api.Data;
using InventoryManagementSoftware.api.Dtos;
using InventoryManagementSoftware.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace InventoryManagementSoftware.api.Services
{
    public class ItemService : IItemService
    {
        private readonly AppDbContext _db;

        public ItemService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<ItemDto>> GetByInventoryIdAsync(Guid inventoryId)
        {
            return await _db.Items
                .AsNoTracking()
                .Include(i => i.CreatedBy)
                .Include(i => i.Comments)
                .Where(i => i.InventoryId == inventoryId)
                .Select(i => new ItemDto
                {
                    Id = i.Id,
                    CustomId = i.CustomId,
                    InventoryId = i.InventoryId,
                    Title = i.Title,
                    Data = i.Data,
                    LikeCount = i.LikeCount,
                    LikedBy = ParseJsonArray(i.LikedBy),
                    CreatedById = i.CreatedById,
                    CreatedByName = i.CreatedBy!.UserName,
                    CreatedAt = i.CreatedAt,
                    UpdatedAt = i.UpdatedAt,
                    RowVersion = Convert.ToBase64String(i.RowVersion),
                    CommentCount = i.Comments.Count
                })
                .ToListAsync();
        }

        public async Task<ItemDto?> GetByIdAsync(Guid id)
        {
            var item = await _db.Items
                .AsNoTracking()
                .Include(i => i.CreatedBy)
                .Include(i => i.Comments)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (item == null) return null;

            return new ItemDto
            {
                Id = item.Id,
                CustomId = item.CustomId,
                InventoryId = item.InventoryId,
                Title = item.Title,
                Data = item.Data,
                LikeCount = item.LikeCount,
                LikedBy = ParseJsonArray(item.LikedBy),
                CreatedById = item.CreatedById,
                CreatedByName = item.CreatedBy?.UserName,
                CreatedAt = item.CreatedAt,
                UpdatedAt = item.UpdatedAt,
                RowVersion = Convert.ToBase64String(item.RowVersion),
                CommentCount = item.Comments.Count
            };
        }

        public async Task<ItemDto> CreateAsync(Guid inventoryId, ItemDto dto)
        {
            var entity = new Item
            {
                Id = Guid.NewGuid(),
                CustomId = dto.CustomId ?? $"ITEM-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}",
                InventoryId = inventoryId,
                Title = dto.Title ?? "Untitled",
                Data = dto.Data,
                CreatedById = dto.CreatedById ?? "system",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                LikeCount = 0,
                LikedBy = "[]"
            };

            _db.Items.Add(entity);
            await _db.SaveChangesAsync();

            var created = await GetByIdAsync(entity.Id);
            return created!;
        }

        public async Task<ItemDto?> UpdateAsync(Guid id, ItemDto dto)
        {
            var entity = await _db.Items
                .Include(i => i.CreatedBy)
                .Include(i => i.Comments)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (entity == null) return null;

            // Check optimistic concurrency: RowVersion from client
            if (!string.IsNullOrEmpty(dto.RowVersion))
            {
                byte[] clientVersion = Convert.FromBase64String(dto.RowVersion);
                if (!clientVersion.SequenceEqual(entity.RowVersion))
                {
                    // Version mismatch — throw concurrency exception
                    throw new DbUpdateConcurrencyException("Item has been modified by another user");
                }
            }

            entity.Title = dto.Title ?? entity.Title;
            entity.Data = dto.Data ?? entity.Data;
            entity.CustomId = dto.CustomId ?? entity.CustomId;
            entity.UpdatedAt = DateTime.UtcNow;

            _db.Items.Update(entity);
            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return new ItemDto
            {
                Id = entity.Id,
                CustomId = entity.CustomId,
                InventoryId = entity.InventoryId,
                Title = entity.Title,
                Data = entity.Data,
                LikeCount = entity.LikeCount,
                LikedBy = ParseJsonArray(entity.LikedBy),
                CreatedById = entity.CreatedById,
                CreatedByName = entity.CreatedBy?.UserName,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                RowVersion = Convert.ToBase64String(entity.RowVersion),
                CommentCount = entity.Comments.Count
            };
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _db.Items.FirstOrDefaultAsync(i => i.Id == id);
            if (entity == null) return false;

            _db.Items.Remove(entity);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> LikeAsync(Guid itemId, string userId)
        {
            var item = await _db.Items.FirstOrDefaultAsync(i => i.Id == itemId);
            if (item == null) return false;

            var likedBy = ParseJsonArray(item.LikedBy);
            if (likedBy.Contains(userId)) return false;

            likedBy.Add(userId);
            item.LikedBy = JsonSerializer.Serialize(likedBy);
            item.LikeCount++;

            _db.Items.Update(item);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UnlikeAsync(Guid itemId, string userId)
        {
            var item = await _db.Items.FirstOrDefaultAsync(i => i.Id == itemId);
            if (item == null) return false;

            var likedBy = ParseJsonArray(item.LikedBy);
            if (!likedBy.Remove(userId)) return false;

            item.LikedBy = JsonSerializer.Serialize(likedBy);
            item.LikeCount = Math.Max(0, item.LikeCount - 1);

            _db.Items.Update(item);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ItemDto>> SearchAsync(string query)
        {
            var lower = query.ToLower();
            return await _db.Items
                .AsNoTracking()
                .Include(i => i.CreatedBy)
                .Include(i => i.Comments)
                .Where(i => i.Title.ToLower().Contains(lower) || 
                           (i.Data != null && i.Data.ToLower().Contains(lower)))
                .Select(i => new ItemDto
                {
                    Id = i.Id,
                    CustomId = i.CustomId,
                    InventoryId = i.InventoryId,
                    Title = i.Title,
                    Data = i.Data,
                    LikeCount = i.LikeCount,
                    LikedBy = ParseJsonArray(i.LikedBy),
                    CreatedById = i.CreatedById,
                    CreatedByName = i.CreatedBy!.UserName,
                    CreatedAt = i.CreatedAt,
                    UpdatedAt = i.UpdatedAt,
                    RowVersion = Convert.ToBase64String(i.RowVersion),
                    CommentCount = i.Comments.Count
                })
                .ToListAsync();
        }

        private static List<string> ParseJsonArray(string json)
        {
            if (string.IsNullOrEmpty(json)) return new();
            try
            {
                return JsonSerializer.Deserialize<List<string>>(json) ?? new();
            }
            catch
            {
                return new();
            }
        }
    }
}
