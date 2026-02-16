namespace InventoryManagementSoftware.api.Dtos
{
    public class LikeDto
    {
        public Guid ItemId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class ToggleLikeRequest
    {
        public Guid ItemId { get; set; }
    }

    public class ToggleLikeResponse
    {
        public bool IsLiked { get; set; }
        public int TotalLikes { get; set; }
    }
}
