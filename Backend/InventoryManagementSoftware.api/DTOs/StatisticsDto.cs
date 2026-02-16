namespace InventoryManagementSoftware.api.Dtos
{
    public class InventoryStatisticsDto
    {
        public int TotalItems { get; set; }
        public decimal CapacityUsedPercent { get; set; }
        public decimal TotalValue { get; set; }
        public Dictionary<string, int> HealthIndexBreakdown { get; set; } = new();
    }

    public class ItemStatisticsDto
    {
        public string FieldName { get; set; } = string.Empty;
        public string FieldType { get; set; } = string.Empty;
        public object? Min { get; set; }
        public object? Max { get; set; }
        public object? Average { get; set; }
        public int Count { get; set; }
    }
}
