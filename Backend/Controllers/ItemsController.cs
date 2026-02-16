using System;
using System.Linq;
using System.Threading.Tasks;
using InventoryManagementSoftware.Api.Data;
using InventoryManagementSoftware.Api.Models;
using InventoryManagementSoftware.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSoftware.Api.Controllers
{
    [ApiController]
    [Route("api/inventories/{inventoryId:guid}/items")]
    public class ItemsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly ICustomIdGenerator _idGenerator;

        public ItemsController(ApplicationDbContext db, ICustomIdGenerator idGenerator)
        {
            _db = db;
            _idGenerator = idGenerator;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(Guid inventoryId, [FromBody] Item create)
        {
            var inventory = await _db.Inventories.FindAsync(inventoryId);
            if (inventory == null) return NotFound();

            // Access control: public vs whitelist
            var userId = User?.Identity?.Name ?? throw new Exception("Unauthorized");
            if (!inventory.IsPublic)
            {
                var allowed = await _db.Set<InventoryWhitelistEntry>().AnyAsync(w => w.InventoryId == inventoryId && w.UserId == userId);
                if (!allowed) return Forbid();
            }

            // Generate custom id (example using {AUTO} placeholder and RandomDigits6)
            var generated = await _idGenerator.GenerateAsync(inventoryId, "{AUTO}", CustomIdStrategy.RandomDigits6);
            create.CustomId = generated;
            create.InventoryId = inventoryId;
            create.CreatedAt = DateTime.UtcNow;
            create.CreatedById = userId;

            _db.Items.Add(create);
            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
            {
                // Custom id conflict
                return Conflict(new { code = "DuplicateCustomId", message = "Generated custom id conflicts. Please provide a different custom id manually." });
            }

            return CreatedAtAction(nameof(Get), new { inventoryId = inventoryId, id = create.Id }, create);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Get(Guid inventoryId, Guid id)
        {
            var item = await _db.Items
                .Include(i => i.Comments)
                .Include(i => i.Likes)
                .FirstOrDefaultAsync(i => i.Id == id && i.InventoryId == inventoryId);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpPut("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> Update(Guid inventoryId, Guid id, [FromBody] Item payload)
        {
            // optimistic concurrency: client must send RowVersion
            var existing = await _db.Items.FirstOrDefaultAsync(i => i.Id == id && i.InventoryId == inventoryId);
            if (existing == null) return NotFound();

            if (payload.RowVersion == null) return BadRequest(new { message = "RowVersion required for optimistic concurrency." });
            // EF will throw DbUpdateConcurrencyException if RowVersion mismatches
            existing.Title = payload.Title;
            existing.Description = payload.Description;
            existing.CustomFields = payload.CustomFields;

            _db.Entry(existing).Property("RowVersion").OriginalValue = payload.RowVersion;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict(new { code = "OptimisticLockFailed", message = "Item was modified by another user. Refresh and retry." });
            }

            return NoContent();
        }

        private static bool IsUniqueConstraintViolation(DbUpdateException ex)
        {
            // Npgsql uses PostgresException; detect unique constraint violation via SqlState 23505
            if (ex.InnerException is PostgresException pg) return pg.SqlState == "23505";
            return false;
        }
    }
}