using System;

namespace InventoryManagementSoftware.api.Dtos
{
    public class ItemDto
    {
        public Guid Id { get; set; }
        public string CustomId { get; set; } = string.Empty;
        public Guid InventoryId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Data { get; set; }
        public int LikeCount { get; set; }
        public List<string> LikedBy { get; set; } = new();
        public string CreatedById { get; set; } = string.Empty;
        public string? CreatedByName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? RowVersion { get; set; }
        public int CommentCount { get; set; }
    }
}

