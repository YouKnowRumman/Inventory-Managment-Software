using InventoryManagementSoftware.api.Dtos;
using InventoryManagementSoftware.api.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace InventoryManagementSoftware.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _service;

        public CommentsController(ICommentService service)
        {
            _service = service;
        }

        [HttpGet("item/{itemId}")]
        public async Task<IActionResult> GetByItemId(Guid itemId)
        {
            try
            {
                var comments = await _service.GetByItemIdAsync(itemId);
                return Ok(comments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to retrieve comments", details = ex.Message });
            }
        }

        [HttpGet("inventory/{inventoryId}")]
        public async Task<IActionResult> GetByInventoryId(Guid inventoryId)
        {
            try
            {
                var comments = await _service.GetByInventoryIdAsync(inventoryId);
                return Ok(comments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to retrieve comments", details = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var comment = await _service.GetByIdAsync(id);
                if (comment == null)
                    return NotFound(new { message = "Comment not found" });

                return Ok(comment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to retrieve comment", details = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CommentDto dto)
        {
            try
            {
                if (dto == null || string.IsNullOrWhiteSpace(dto.Text))
                    return BadRequest(new { message = "Text is required" });

                if (dto.ItemId == null && dto.InventoryId == null)
                    return BadRequest(new { message = "Either ItemId or InventoryId is required" });

                var created = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to create comment", details = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CommentDto dto)
        {
            try
            {
                var updated = await _service.UpdateAsync(id, dto);
                if (updated == null)
                    return NotFound(new { message = "Comment not found" });

                return Ok(updated);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to update comment", details = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var deleted = await _service.DeleteAsync(id);
                if (!deleted)
                    return NotFound(new { message = "Comment not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to delete comment", details = ex.Message });
            }
        }
    }
}
