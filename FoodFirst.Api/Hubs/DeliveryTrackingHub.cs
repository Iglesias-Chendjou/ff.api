using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace FoodFirst.Api.Hubs;

[Authorize]
public class DeliveryTrackingHub : Hub
{
    public Task JoinDeliveryGroup(Guid deliveryId) =>
        Groups.AddToGroupAsync(Context.ConnectionId, $"delivery-{deliveryId}");

    public Task LeaveDeliveryGroup(Guid deliveryId) =>
        Groups.RemoveFromGroupAsync(Context.ConnectionId, $"delivery-{deliveryId}");

    public Task BroadcastLocation(Guid deliveryId, decimal latitude, decimal longitude) =>
        Clients.Group($"delivery-{deliveryId}").SendAsync("LocationUpdated", deliveryId, latitude, longitude);
}
