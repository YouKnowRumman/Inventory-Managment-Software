using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagementSoftware.Domain.Entities
{
    public class Inventory
    {
        public Guid Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? Category { get; set; }

        public string? ImageUrl { get; set; }

        public string OwnerId { get; set; } = string.Empty;

        public ApplicationUser? Owner { get; set; }

        public bool IsPublic { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Timestamp]
        public byte[] RowVersion { get; set; } = null!;

        // Tags stored as JSON array
        [Column(TypeName = "jsonb")]
        public string Tags { get; set; } = "[]";

        // JSONB stored field definitions (array of field definition objects)
        [Column(TypeName = "jsonb")]
        public string? FieldDefinitions { get; set; }

        // Custom ID template configuration
        [Column(TypeName = "jsonb")]
        public string? CustomIdTemplate { get; set; }

        // Access control: list of user IDs with write access (comma-separated or JSON array)
        [Column(TypeName = "jsonb")]
        public string AccessList { get; set; } = "[]";

        // Navigation properties
        public ICollection<Item> Items { get; set; } = new List<Item>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
