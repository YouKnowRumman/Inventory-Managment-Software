using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSoftware.Domain.Entities
{
    public class Comment
    {
        public Guid Id { get; set; }

        public Guid? ItemId { get; set; }

        public Item? Item { get; set; }

        public Guid? InventoryId { get; set; }

        public Inventory? Inventory { get; set; }

        [Required]
        public string Text { get; set; } = string.Empty;

        public string CreatedById { get; set; } = string.Empty;

        public ApplicationUser? CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
