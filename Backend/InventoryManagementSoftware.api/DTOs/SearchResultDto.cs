namespace InventoryManagementSoftware.api.Dtos
{
    public class SearchResultDto
    {
        public string Type { get; set; } = string.Empty; // "inventory" or "item"
        public Guid Id { get; set; }
        public Guid? InventoryId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Category { get; set; }
        public string? CreatorName { get; set; }
        public int? ItemCount { get; set; }
        public double RelevanceScore { get; set; }
    }

    public class GlobalSearchRequest
    {
        public string Query { get; set; } = string.Empty;
        public int Limit { get; set; } = 20;
        public string? Type { get; set; } // "all", "inventory", or "item"
    }
}
