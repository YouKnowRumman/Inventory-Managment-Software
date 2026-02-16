using InventoryManagementSoftware.api.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryManagementSoftware.api.Services
{
    public interface ICommentService
    {
        Task<IEnumerable<CommentDto>> GetByItemIdAsync(Guid itemId);
        Task<IEnumerable<CommentDto>> GetByInventoryIdAsync(Guid inventoryId);
        Task<CommentDto?> GetByIdAsync(Guid id);
        Task<CommentDto> CreateAsync(CommentDto dto);
        Task<CommentDto?> UpdateAsync(Guid id, CommentDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
