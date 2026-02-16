using InventoryManagementSoftware.api.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryManagementSoftware.api.Services
{
    public interface ILikeService
    {
        Task<IEnumerable<LikeDto>> GetLikesByItemAsync(Guid itemId);
        Task<bool> IsLikedByUserAsync(Guid itemId, string userId);
        Task<ToggleLikeResponse> ToggleLikeAsync(Guid itemId, string userId, string userName);
        Task<int> GetLikeCountAsync(Guid itemId);
    }
}
