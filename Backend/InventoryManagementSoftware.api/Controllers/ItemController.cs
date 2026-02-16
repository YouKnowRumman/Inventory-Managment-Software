using InventoryManagementSoftware.api.Dtos;
using InventoryManagementSoftware.api.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace InventoryManagementSoftware.api.Controllers
{
    [ApiController]
    [Route("api/inventories/{inventoryId}/items")]
    public class ItemController : ControllerBase
    {
        private readonly IItemService _service;

        public ItemController(IItemService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetByInventoryId(Guid inventoryId)
        {
            try
            {
                var items = await _service.GetByInventoryIdAsync(inventoryId);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to retrieve items", details = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid inventoryId, Guid id)
        {
            try
            {
                var item = await _service.GetByIdAsync(id);
                if (item == null) return NotFound(new { message = "Item not found" });
                if (item.InventoryId != inventoryId) return BadRequest(new { message = "Item does not belong to this inventory" });
                return Ok(item);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to retrieve item", details = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(Guid inventoryId, [FromBody] ItemDto dto)
        {
            try
            {
                if (dto == null || string.IsNullOrWhiteSpace(dto.Title))
                    return BadRequest(new { message = "Title is required" });

                var created = await _service.CreateAsync(inventoryId, dto);
                return CreatedAtAction(nameof(GetById), new { inventoryId, id = created.Id }, created);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to create item", details = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid inventoryId, Guid id, [FromBody] ItemDto dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest(new { message = "Request body is required" });

                var updated = await _service.UpdateAsync(id, dto);
                if (updated == null)
                    return NotFound(new { message = "Item not found" });

                return Ok(updated);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException)
            {
                return Conflict(new { message = "Item has been modified by another user. Please refresh and try again." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to update item", details = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid inventoryId, Guid id)
        {
            try
            {
                var deleted = await _service.DeleteAsync(id);
                if (!deleted)
                    return NotFound(new { message = "Item not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to delete item", details = ex.Message });
            }
        }

        [HttpPost("{id}/like")]
        public async Task<IActionResult> Like(Guid inventoryId, Guid id)
        {
            try
            {
                // In a real app, get userId from claims
                var liked = await _service.LikeAsync(id, "anonymous");
                if (!liked)
                    return NotFound(new { message = "Item not found" });

                return Ok(new { message = "Liked" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to like item", details = ex.Message });
            }
        }

        [HttpPost("{id}/unlike")]
        public async Task<IActionResult> Unlike(Guid inventoryId, Guid id)
        {
            try
            {
                var unliked = await _service.UnlikeAsync(id, "anonymous");
                if (!unliked)
                    return NotFound(new { message = "Item not found" });

                return Ok(new { message = "Unliked" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to unlike item", details = ex.Message });
            }
        }
    }
}
