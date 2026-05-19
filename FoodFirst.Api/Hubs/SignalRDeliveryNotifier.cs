using FoodFirst.Service.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace FoodFirst.Api.Hubs;

public class SignalRDeliveryNotifier(IHubContext<DeliveryTrackingHub> hub) : IDeliveryNotifier
{
    public Task LocationUpdatedAsync(Guid deliveryId, decimal latitude, decimal longitude, CancellationToken ct = default) =>
        hub.Clients.Group($"delivery-{deliveryId}")
            .SendAsync("LocationUpdated", new { deliveryId, latitude, longitude, at = DateTime.UtcNow }, ct);

    public Task StatusChangedAsync(Guid deliveryId, string status, CancellationToken ct = default) =>
        hub.Clients.Group($"delivery-{deliveryId}")
            .SendAsync("StatusChanged", new { deliveryId, status, at = DateTime.UtcNow }, ct);
}
