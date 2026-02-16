using InventoryManagementSoftware.api.Dtos;
using InventoryManagementSoftware.api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace InventoryManagementSoftware.Api.Controllers
{
    [ApiController]
    [Route("api/inventories/{inventoryId:guid}/custom-id")]
    [Authorize]
    public class CustomIdController : ControllerBase
    {
        private readonly ICustomIdGeneratorService _service;

        public CustomIdController(ICustomIdGeneratorService service)
        {
            _service = service;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateCustomId(Guid inventoryId, [FromBody] CustomIdGenerationDto dto)
        {
            if (dto == null)
                return BadRequest(new { message = "Custom ID generation configuration is required" });

            try
            {
                var generatedId = await _service.GenerateCustomIdAsync(inventoryId, dto);
                return Ok(new GeneratedCustomIdResponse { GeneratedId = generatedId });
            }
            catch (Exception ex) when (ex.Message.Contains("conflicts"))
            {
                return Conflict(new CustomIdConflictError
                {
                    Code = "DuplicateCustomId",
                    Message = "Generated custom ID conflicts. Please provide a different custom ID manually."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to generate custom ID", details = ex.Message });
            }
        }

        [HttpPost("check-unique")]
        public async Task<IActionResult> CheckUniqueCustomId(Guid inventoryId, [FromQuery] string customId)
        {
            if (string.IsNullOrEmpty(customId))
                return BadRequest(new { message = "CustomId query parameter is required" });

            try
            {
                var isUnique = await _service.IsCustomIdUniqueAsync(inventoryId, customId);
                return Ok(new { isUnique });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to check uniqueness", details = ex.Message });
            }
        }
    }
}
