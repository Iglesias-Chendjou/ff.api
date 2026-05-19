namespace FoodFirst.Service.Interfaces;

public interface IDeliveryNotifier
{
    Task LocationUpdatedAsync(Guid deliveryId, decimal latitude, decimal longitude, CancellationToken ct = default);
    Task StatusChangedAsync(Guid deliveryId, string status, CancellationToken ct = default);
}
