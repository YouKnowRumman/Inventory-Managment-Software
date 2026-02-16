using InventoryManagementSoftware.api.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryManagementSoftware.api.Services
{
    public interface ICustomIdGeneratorService
    {
        Task<string> GenerateCustomIdAsync(Guid inventoryId, CustomIdGenerationDto dto);
        Task<bool> IsCustomIdUniqueAsync(Guid inventoryId, string customId);
    }
}
