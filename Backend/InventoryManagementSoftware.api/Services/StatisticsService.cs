using InventoryManagementSoftware.api.Dtos;
using InventoryManagementSoftware.api.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSoftware.api.Services
{
    public class StatisticsService : IStatisticsService
    {
        private readonly AppDbContext _context;

        public StatisticsService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<InventoryStatisticsDto> GetInventoryStatisticsAsync(Guid inventoryId)
        {
            var inventory = await _context.Inventories.FindAsync(inventoryId);
            if (inventory == null)
                throw new Exception("Inventory not found");

            var items = await _context.Items
                .Where(i => i.InventoryId == inventoryId)
                .ToListAsync();

            int totalItems = items.Count;
            decimal capacityUsed = totalItems > 0 ? (totalItems / 1000.0m) * 100 : 0;

            // Calculate total value from numeric fields
            decimal totalValue = 0;
            if (!string.IsNullOrEmpty(inventory.FieldDefinitions))
            {
                try
                {
                    var fieldDefs = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(inventory.FieldDefinitions) ?? new List<Dictionary<string, object>>();
                    var numericFields = fieldDefs
                        .Where(f => f.ContainsKey("type") && f["type"]?.ToString() == "number")
                        .Select(f => f.ContainsKey("id") ? f["id"]?.ToString() : null)
                        .Where(id => !string.IsNullOrEmpty(id))
                        .ToList();

                    foreach (var item in items)
                    {
                        if (!string.IsNullOrEmpty(item.Data))
                        {
                            var data = JsonSerializer.Deserialize<Dictionary<string, object>>(item.Data);
                            foreach (var field in numericFields)
                            {
                                if (data?.TryGetValue(field, out var value) == true)
                                {
                                    if (decimal.TryParse(value?.ToString(), out var numValue))
                                    {
                                        totalValue += numValue;
                                    }
                                }
                            }
                        }
                    }
                }
                catch { /* Ignore JSON parsing errors */ }
            }

            // Health index breakdown based on operational state field
            var healthIndex = new Dictionary<string, int>
            {
                { "Optimal", 0 },
                { "Service Needed", 0 },
                { "Decommissioned", 0 }
            };

            foreach (var item in items)
            {
                if (!string.IsNullOrEmpty(item.Data))
                {
                    try
                    {
                        var data = JsonSerializer.Deserialize<Dictionary<string, object>>(item.Data);
                        if (data?.TryGetValue("f3", out var state) == true)
                        {
                            string stateStr = state?.ToString() ?? "Optimal";
                            if (healthIndex.ContainsKey(stateStr))
                                healthIndex[stateStr]++;
                            else
                                healthIndex["Optimal"]++;
                        }
                        else
                        {
                            healthIndex["Optimal"]++;
                        }
                    }
                    catch { healthIndex["Optimal"]++; }
                }
                else
                {
                    healthIndex["Optimal"]++;
                }
            }

            return new InventoryStatisticsDto
            {
                TotalItems = totalItems,
                CapacityUsedPercent = capacityUsed,
                TotalValue = totalValue,
                HealthIndexBreakdown = healthIndex
            };
        }

        public async Task<IEnumerable<ItemStatisticsDto>> GetItemFieldStatisticsAsync(Guid inventoryId, string fieldName)
        {
            var items = await _context.Items
                .Where(i => i.InventoryId == inventoryId)
                .ToListAsync();

            var statistics = new List<ItemStatisticsDto>();
            var numericValues = new List<decimal>();

            foreach (var item in items)
            {
                if (!string.IsNullOrEmpty(item.Data))
                {
                    try
                    {
                        var data = JsonSerializer.Deserialize<Dictionary<string, object>>(item.Data);
                        if (data?.TryGetValue(fieldName, out var value) == true && value != null)
                        {
                            if (decimal.TryParse(value.ToString(), out var numValue))
                            {
                                numericValues.Add(numValue);
                            }
                        }
                    }
                    catch { /* Skip parsing errors */ }
                }
            }

            if (numericValues.Count > 0)
            {
                statistics.Add(new ItemStatisticsDto
                {
                    FieldName = fieldName,
                    FieldType = "number",
                    Min = numericValues.Min(),
                    Max = numericValues.Max(),
                    Average = numericValues.Average(),
                    Count = numericValues.Count
                });
            }

            return statistics;
        }
    }
}
