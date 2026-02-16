using InventoryManagementSoftware.api.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryManagementSoftware.api.Services
{
    public interface IInventoryService
    {
        Task<IEnumerable<InventoryDto>> GetAllAsync();
        Task<InventoryDto?> GetByIdAsync(Guid id);
        Task<InventoryDto> CreateAsync(InventoryDto dto);
        Task<InventoryDto> UpdateAsync(Guid id, InventoryDto dto);
        Task<bool> DeleteAsync(Guid id);
        Task<IEnumerable<InventoryDto>> SearchAsync(string query);
    }
}

