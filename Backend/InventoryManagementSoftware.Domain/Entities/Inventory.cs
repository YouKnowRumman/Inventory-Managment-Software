using System;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSoftware.Domain.Entities
{
    public class Inventory
    {
        public Guid Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string OwnerId { get; set; } = string.Empty;

        // Navigation property omitted in domain model to avoid dependency on ASP.NET Identity types
        public bool IsPublic { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; } = null!;

        // JSONB stored field definitions
        public string? FieldDefinitions { get; set; }
    }
}
