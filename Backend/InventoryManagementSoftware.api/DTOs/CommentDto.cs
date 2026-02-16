using System;

namespace InventoryManagementSoftware.api.Dtos
{
    public class CommentDto
    {
        public Guid Id { get; set; }
        public Guid? ItemId { get; set; }
        public Guid? InventoryId { get; set; }
        public string Text { get; set; } = string.Empty;
        public string CreatedById { get; set; } = string.Empty;
        public string? CreatedByName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
