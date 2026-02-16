using InventoryManagementSoftware.api.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryManagementSoftware.api.Services
{
    public interface IItemService
    {
        Task<IEnumerable<ItemDto>> GetByInventoryIdAsync(Guid inventoryId);
        Task<ItemDto?> GetByIdAsync(Guid id);
        Task<ItemDto> CreateAsync(Guid inventoryId, ItemDto dto);
        Task<ItemDto?> UpdateAsync(Guid id, ItemDto dto);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> LikeAsync(Guid itemId, string userId);
        Task<bool> UnlikeAsync(Guid itemId, string userId);
        Task<IEnumerable<ItemDto>> SearchAsync(string query);
    }
}

