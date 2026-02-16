using InventoryManagementSoftware.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryManagementSoftware.Application.Services
{
    public interface IInventoryService
    {
        Task<IEnumerable<InventoryDto>> GetAllAsync();
        Task<InventoryDto?> GetByIdAsync(Guid id);
        Task<InventoryDto> CreateAsync(InventoryDto dto);
    }
}
