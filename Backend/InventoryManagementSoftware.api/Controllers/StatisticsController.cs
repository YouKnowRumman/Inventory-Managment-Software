using InventoryManagementSoftware.api.Dtos;
using InventoryManagementSoftware.api.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryManagementSoftware.Api.Controllers
{
    [ApiController]
    [Route("api/inventories/{inventoryId:guid}/statistics")]
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticsService _service;

        public StatisticsController(IStatisticsService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetInventoryStatistics(Guid inventoryId)
        {
            try
            {
                var stats = await _service.GetInventoryStatisticsAsync(inventoryId);
                return Ok(stats);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = "Inventory not found or statistics unavailable", details = ex.Message });
            }
        }

        [HttpGet("fields/{fieldName}")]
        public async Task<IActionResult> GetFieldStatistics(Guid inventoryId, string fieldName)
        {
            try
            {
                var stats = await _service.GetItemFieldStatisticsAsync(inventoryId, fieldName);
                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to calculate field statistics", details = ex.Message });
            }
        }
    }
}
