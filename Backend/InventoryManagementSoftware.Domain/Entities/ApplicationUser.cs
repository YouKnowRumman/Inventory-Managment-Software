using Microsoft.AspNetCore.Identity;

namespace InventoryManagementSoftware.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public bool IsBlocked { get; set; }
        public string? Language { get; set; }
        public string? Theme { get; set; }
    }
}
