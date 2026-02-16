using InventoryManagementSoftware.api.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryManagementSoftware.api.Services
{
    public interface IStatisticsService
    {
        Task<InventoryStatisticsDto> GetInventoryStatisticsAsync(Guid inventoryId);
        Task<IEnumerable<ItemStatisticsDto>> GetItemFieldStatisticsAsync(Guid inventoryId, string fieldName);
    }
}
