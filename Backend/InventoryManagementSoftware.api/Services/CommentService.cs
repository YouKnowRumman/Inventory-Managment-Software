using InventoryManagementSoftware.api.Data;
using InventoryManagementSoftware.api.Dtos;
using InventoryManagementSoftware.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSoftware.api.Services
{
    public class CommentService : ICommentService
    {
        private readonly AppDbContext _db;

        public CommentService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<CommentDto>> GetByItemIdAsync(Guid itemId)
        {
            return await _db.Comments
                .AsNoTracking()
                .Include(c => c.CreatedBy)
                .Where(c => c.ItemId == itemId)
                .OrderBy(c => c.CreatedAt)
                .Select(c => new CommentDto
                {
                    Id = c.Id,
                    ItemId = c.ItemId,
                    InventoryId = c.InventoryId,
                    Text = c.Text,
                    CreatedById = c.CreatedById,
                    CreatedByName = c.CreatedBy!.UserName,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<CommentDto>> GetByInventoryIdAsync(Guid inventoryId)
        {
            return await _db.Comments
                .AsNoTracking()
                .Include(c => c.CreatedBy)
                .Where(c => c.InventoryId == inventoryId)
                .OrderBy(c => c.CreatedAt)
                .Select(c => new CommentDto
                {
                    Id = c.Id,
                    ItemId = c.ItemId,
                    InventoryId = c.InventoryId,
                    Text = c.Text,
                    CreatedById = c.CreatedById,
                    CreatedByName = c.CreatedBy!.UserName,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                })
                .ToListAsync();
        }

        public async Task<CommentDto?> GetByIdAsync(Guid id)
        {
            var comment = await _db.Comments
                .AsNoTracking()
                .Include(c => c.CreatedBy)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (comment == null) return null;

            return new CommentDto
            {
                Id = comment.Id,
                ItemId = comment.ItemId,
                InventoryId = comment.InventoryId,
                Text = comment.Text,
                CreatedById = comment.CreatedById,
                CreatedByName = comment.CreatedBy?.UserName,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt
            };
        }

        public async Task<CommentDto> CreateAsync(CommentDto dto)
        {
            var entity = new Comment
            {
                Id = Guid.NewGuid(),
                ItemId = dto.ItemId,
                InventoryId = dto.InventoryId,
                Text = dto.Text,
                CreatedById = dto.CreatedById,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _db.Comments.Add(entity);
            await _db.SaveChangesAsync();

            return await GetByIdAsync(entity.Id) ?? dto;
        }

        public async Task<CommentDto?> UpdateAsync(Guid id, CommentDto dto)
        {
            var entity = await _db.Comments
                .Include(c => c.CreatedBy)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (entity == null) return null;

            entity.Text = dto.Text ?? entity.Text;
            entity.UpdatedAt = DateTime.UtcNow;

            _db.Comments.Update(entity);
            await _db.SaveChangesAsync();

            return await GetByIdAsync(entity.Id);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _db.Comments.FirstOrDefaultAsync(c => c.Id == id);
            if (entity == null) return false;

            _db.Comments.Remove(entity);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
