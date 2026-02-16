using System;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSoftware.Domain.Entities
{
    public class Item
    {
        public Guid Id { get; set; }

        [Required]
        public string CustomId { get; set; } = string.Empty;

        public Guid InventoryId { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; } = null!;

        // Flexible data for custom fields (stored as jsonb in PostgreSQL)
        public string? Data { get; set; }
    }
}
