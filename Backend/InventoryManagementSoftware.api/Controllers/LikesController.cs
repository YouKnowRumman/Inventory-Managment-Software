using InventoryManagementSoftware.api.Dtos;
using InventoryManagementSoftware.api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace InventoryManagementSoftware.Api.Controllers
{
    [ApiController]
    [Route("api/items/{itemId:guid}/likes")]
    public class LikesController : ControllerBase
    {
        private readonly ILikeService _service;

        public LikesController(ILikeService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetLikes(Guid itemId)
        {
            try
            {
                var likes = await _service.GetLikesByItemAsync(itemId);
                return Ok(likes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to fetch likes", details = ex.Message });
            }
        }

        [HttpPost("toggle")]
        [Authorize]
        public async Task<IActionResult> ToggleLike(Guid itemId)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userName = User.FindFirst(ClaimTypes.Name)?.Value ?? "Anonymous";

                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { message = "User ID not found" });

                var result = await _service.ToggleLikeAsync(itemId, userId, userName);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to toggle like", details = ex.Message });
            }
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetLikeCount(Guid itemId)
        {
            try
            {
                var count = await _service.GetLikeCountAsync(itemId);
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to get like count", details = ex.Message });
            }
        }
    }
}
