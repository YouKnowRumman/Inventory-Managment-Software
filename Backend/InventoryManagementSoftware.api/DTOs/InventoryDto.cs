using System;

namespace InventoryManagementSoftware.api.Dtos
{
    public class InventoryDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Category { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsPublic { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<string> Tags { get; set; } = new();
        public string? FieldDefinitions { get; set; }
        public string? CustomIdTemplate { get; set; }
        public List<string> AccessList { get; set; } = new();
        public int ItemCount { get; set; }
        public string? RowVersion { get; set; }
        public string? OwnerName { get; set; }
    }
}

