namespace InventoryManagementSoftware.api.Dtos
{
    public class CustomIdGenerationDto
    {
        public string? Format { get; set; } // Template string, e.g., "PREFIX-{YEAR}-{SEQ}"
        public string Strategy { get; set; } = "FixedText"; // FixedText, RandomBits20, RandomBits32, RandomDigits6, RandomDigits9, Guid, DateTime, Sequence
    }

    public class GeneratedCustomIdResponse
    {
        public string GeneratedId { get; set; } = string.Empty;
    }

    public class CustomIdConflictError
    {
        public string Code { get; set; } = "DuplicateCustomId";
        public string Message { get; set; } = "Generated custom ID conflicts. Please provide a different custom ID manually.";
    }
}
