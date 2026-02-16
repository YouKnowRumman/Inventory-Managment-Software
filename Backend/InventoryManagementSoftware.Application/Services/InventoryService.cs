using InventoryManagementSoftware.Application.Dtos;
using InventoryManagementSoftware.Domain.Entities;
using InventoryManagementSoftware.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSoftware.Application.Services
{
    public class InventoryService : IInventoryService
    {
        // FIX: Use the short name 'AppDbContext' because of the 'using' statement above
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
                    IsPublic = i.IsPublic,
                    FieldDefinitions = i.FieldDefinitions
                })
                .ToListAsync();
        }

        public async Task<InventoryDto?> GetByIdAsync(Guid id)
        {
            var inv = await _db.Inventories
                .AsNoTracking()
                .Where(i => i.Id == id)
                .Select(i => new InventoryDto
                {
                    Id = i.Id,
                    Title = i.Title,
                    Description = i.Description,
                    IsPublic = i.IsPublic,
                    FieldDefinitions = i.FieldDefinitions
                })
                .FirstOrDefaultAsync();

            return inv;
        }

        public async Task<InventoryDto> CreateAsync(InventoryDto dto)
        {
            var entity = new Inventory
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Description = dto.Description,
                IsPublic = dto.IsPublic,
                FieldDefinitions = dto.FieldDefinitions
            };

            _db.Inventories.Add(entity);
            await _db.SaveChangesAsync();

            dto.Id = entity.Id;
            return dto;
        }
    }
}