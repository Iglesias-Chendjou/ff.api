using FoodFirst.Dto.Deliveries;

namespace FoodFirst.Service.Interfaces;

public interface IDeliveryService
{
    Task<IReadOnlyList<DeliveryDto>> GetMineAsync(Guid deliveryPersonId, CancellationToken ct = default);
    Task PickupAsync(Guid deliveryId, CancellationToken ct = default);
    Task UpdateLocationAsync(Guid deliveryId, UpdateLocationRequest request, CancellationToken ct = default);
    Task CompleteAsync(Guid deliveryId, CompleteDeliveryRequest request, CancellationToken ct = default);
    Task FailAsync(Guid deliveryId, FailDeliveryRequest request, CancellationToken ct = default);
}
