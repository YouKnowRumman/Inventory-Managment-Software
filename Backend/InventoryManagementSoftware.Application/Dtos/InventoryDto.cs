using System;

namespace InventoryManagementSoftware.Application.Dtos
{
    public class InventoryDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsPublic { get; set; }
        public string? FieldDefinitions { get; set; }
    }
}
