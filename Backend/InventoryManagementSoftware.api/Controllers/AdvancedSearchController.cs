using InventoryManagementSoftware.api.Dtos;
using InventoryManagementSoftware.api.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryManagementSoftware.Api.Controllers
{
    [ApiController]
    [Route("api/search")]
    public class AdvancedSearchController : ControllerBase
    {
        private readonly ISearchService _service;

        public AdvancedSearchController(ISearchService service)
        {
            _service = service;
        }

        [HttpGet("global")]
        public async Task<IActionResult> GlobalSearch([FromQuery] string query, [FromQuery] int limit = 20)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest(new { message = "Query parameter is required" });

            try
            {
                var results = await _service.GlobalSearchAsync(query, limit);
                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Search failed", details = ex.Message });
            }
        }

        [HttpGet("inventories")]
        public async Task<IActionResult> SearchInventories([FromQuery] string query, [FromQuery] int limit = 20)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest(new { message = "Query parameter is required" });

            try
            {
                var results = await _service.SearchInventoriesAsync(query, limit);
                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Search failed", details = ex.Message });
            }
        }

        [HttpGet("items")]
        public async Task<IActionResult> SearchItems([FromQuery] string query, [FromQuery] int limit = 20)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest(new { message = "Query parameter is required" });

            try
            {
                var results = await _service.SearchItemsAsync(query, limit);
                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Search failed", details = ex.Message });
            }
        }

        [HttpGet("tags/autocomplete")]
        public async Task<IActionResult> AutocompleteTags([FromQuery] string prefix, [FromQuery] int limit = 10)
        {
            if (string.IsNullOrWhiteSpace(prefix))
                return BadRequest(new { message = "Prefix parameter is required" });

            try
            {
                var tags = await _service.AutocompleteTagsAsync(prefix, limit);
                return Ok(tags);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Autocomplete failed", details = ex.Message });
            }
        }
    }
}
