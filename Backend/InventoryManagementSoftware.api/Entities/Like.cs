using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagementSoftware.Domain.Entities
{
    /// <summary>
    /// Represents a like on an item by a user.
    /// Composite key: (ItemId, UserId) ensures one like per user per item.
    /// </summary>
    public class Like
    {
        [Key]
        [Column(Order = 0)]
        public Guid ItemId { get; set; }

        [Key]
        [Column(Order = 1)]
        public string UserId { get; set; } = string.Empty;

        public Item Item { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
