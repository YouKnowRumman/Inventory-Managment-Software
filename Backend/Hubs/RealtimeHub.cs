using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace InventoryManagementSoftware.Api.Hubs
{
    [Authorize]
    public class RealtimeHub : Hub
    {
        // Clients subscribe to inventory rooms: "inventory:{inventoryId}"
        public Task JoinInventory(string inventoryId) => Groups.AddToGroupAsync(Context.ConnectionId, $"inventory:{inventoryId}");
        public Task LeaveInventory(string inventoryId) => Groups.RemoveFromGroupAsync(Context.ConnectionId, $"inventory:{inventoryId}");

        // Server will Broadcast comment/like events to groups
        public Task BroadcastComment(string inventoryId, object comment) => Clients.Group($"inventory:{inventoryId}").SendAsync("comment", comment);
        public Task BroadcastLike(string inventoryId, object like) => Clients.Group($"inventory:{inventoryId}").SendAsync("like", like);
    }
}