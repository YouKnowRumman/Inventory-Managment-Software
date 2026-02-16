using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagementSoftware.Domain.Entities
{
    public class Item
    {
        public Guid Id { get; set; }

        [Required]
        public string CustomId { get; set; } = string.Empty;

        public Guid InventoryId { get; set; }

        public Inventory Inventory { get; set; } = null!;

        [Required]
        public string Title { get; set; } = string.Empty;

        public string CreatedById { get; set; } = string.Empty;

        public ApplicationUser? CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Timestamp]
        public byte[] RowVersion { get; set; } = null!;

        // Flexible data for custom fields (stored as jsonb in PostgreSQL)
        [Column(TypeName = "jsonb")]
        public string? Data { get; set; }

        // Likes count
        public int LikeCount { get; set; } = 0;

        // Users who liked (stored as JSON array of user IDs)
        [Column(TypeName = "jsonb")]
        public string LikedBy { get; set; } = "[]";

        // Navigation properties
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
