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
    public class InventoryService : IInventoryService
    {
        private readonly AppDbContext _db;

        public InventoryService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<InventoryDto>> GetAllAsync()
        {
            return await _db.Inventories
                .AsNoTracking()
                .Select(i => new InventoryDto
                {
                    Id = i.Id,
                    Title = i.Title,
                    Description = i.Description,
                    Category = i.Category,
                    ImageUrl = i.ImageUrl,
                    IsPublic = i.IsPublic,
                    CreatedAt = i.CreatedAt,
                    UpdatedAt = i.UpdatedAt,
                    Tags = ParseJsonArray(i.Tags),
                    FieldDefinitions = i.FieldDefinitions,
                    CustomIdTemplate = i.CustomIdTemplate,
                    AccessList = ParseJsonArray(i.AccessList),
                    ItemCount = i.Items.Count,
                    RowVersion = Convert.ToBase64String(i.RowVersion),
                    OwnerName = i.Owner!.UserName ?? "Unknown"
                })
                .ToListAsync();
        }

        public async Task<InventoryDto?> GetByIdAsync(Guid id)
        {
            var inv = await _db.Inventories
                .AsNoTracking()
                .Include(i => i.Owner)
                .Include(i => i.Items)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (inv == null) return null;

            return new InventoryDto
            {
                Id = inv.Id,
                Title = inv.Title,
                Description = inv.Description,
                Category = inv.Category,
                ImageUrl = inv.ImageUrl,
                IsPublic = inv.IsPublic,
                CreatedAt = inv.CreatedAt,
                UpdatedAt = inv.UpdatedAt,
                Tags = ParseJsonArray(inv.Tags),
                FieldDefinitions = inv.FieldDefinitions,
                CustomIdTemplate = inv.CustomIdTemplate,
                AccessList = ParseJsonArray(inv.AccessList),
                ItemCount = inv.Items.Count,
                RowVersion = Convert.ToBase64String(inv.RowVersion),
                OwnerName = inv.Owner?.UserName ?? "Unknown"
            };
        }

        public async Task<InventoryDto> CreateAsync(InventoryDto dto)
        {
            var entity = new Inventory
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Description = dto.Description,
                Category = dto.Category,
                ImageUrl = dto.ImageUrl,
                IsPublic = dto.IsPublic,
                Tags = JsonSerializer.Serialize(dto.Tags ?? new()),
                FieldDefinitions = dto.FieldDefinitions,
                CustomIdTemplate = dto.CustomIdTemplate,
                AccessList = JsonSerializer.Serialize(dto.AccessList ?? new()),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _db.Inventories.Add(entity);
            await _db.SaveChangesAsync();

            dto.Id = entity.Id;
            dto.CreatedAt = entity.CreatedAt;
            dto.UpdatedAt = entity.UpdatedAt;
            dto.RowVersion = Convert.ToBase64String(entity.RowVersion);
            return dto;
        }

        public async Task<InventoryDto> UpdateAsync(Guid id, InventoryDto dto)
        {
            var entity = await _db.Inventories
                .Include(i => i.Owner)
                .Include(i => i.Items)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (entity == null)
                throw new KeyNotFoundException($"Inventory {id} not found");

            entity.Title = dto.Title ?? entity.Title;
            entity.Description = dto.Description ?? entity.Description;
            entity.Category = dto.Category ?? entity.Category;
            entity.ImageUrl = dto.ImageUrl ?? entity.ImageUrl;
            entity.IsPublic = dto.IsPublic;
            entity.Tags = JsonSerializer.Serialize(dto.Tags ?? new());
            entity.FieldDefinitions = dto.FieldDefinitions ?? entity.FieldDefinitions;
            entity.CustomIdTemplate = dto.CustomIdTemplate ?? entity.CustomIdTemplate;
            entity.AccessList = JsonSerializer.Serialize(dto.AccessList ?? new());
            entity.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return new InventoryDto
            {
                Id = entity.Id,
                Title = entity.Title,
                Description = entity.Description,
                Category = entity.Category,
                ImageUrl = entity.ImageUrl,
                IsPublic = entity.IsPublic,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                Tags = ParseJsonArray(entity.Tags),
                FieldDefinitions = entity.FieldDefinitions,
                CustomIdTemplate = entity.CustomIdTemplate,
                AccessList = ParseJsonArray(entity.AccessList),
                ItemCount = entity.Items.Count,
                RowVersion = Convert.ToBase64String(entity.RowVersion),
                OwnerName = entity.Owner?.UserName ?? "Unknown"
            };
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _db.Inventories.FirstOrDefaultAsync(i => i.Id == id);
            if (entity == null) return false;

            _db.Inventories.Remove(entity);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<InventoryDto>> SearchAsync(string query)
        {
            var lower = query.ToLower();
            return await _db.Inventories
                .AsNoTracking()
                .Include(i => i.Items)
                .Where(i => i.Title.ToLower().Contains(lower) || 
                           i.Description!.ToLower().Contains(lower))
                .Select(i => new InventoryDto
                {
                    Id = i.Id,
                    Title = i.Title,
                    Description = i.Description,
                    Category = i.Category,
                    ImageUrl = i.ImageUrl,
                    IsPublic = i.IsPublic,
                    CreatedAt = i.CreatedAt,
                    UpdatedAt = i.UpdatedAt,
                    Tags = ParseJsonArray(i.Tags),
                    FieldDefinitions = i.FieldDefinitions,
                    CustomIdTemplate = i.CustomIdTemplate,
                    AccessList = ParseJsonArray(i.AccessList),
                    ItemCount = i.Items.Count,
                    RowVersion = Convert.ToBase64String(i.RowVersion),
                    OwnerName = i.Owner!.UserName ?? "Unknown"
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

