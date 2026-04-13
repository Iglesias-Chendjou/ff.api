using FoodFirst.Dal.Enums;
using FoodFirst.Dto.Deliveries;
using FoodFirst.Repository.Interfaces;
using FoodFirst.Service.Interfaces;

namespace FoodFirst.Service.Implementations;

public class DeliveryService(
    IDeliveryRepository deliveries,
    IOrderRepository orders) : IDeliveryService
{
    public async Task<IReadOnlyList<DeliveryDto>> GetMineAsync(Guid deliveryPersonId, CancellationToken ct = default)
    {
        var list = await deliveries.GetByDeliveryPersonAsync(deliveryPersonId, ct);
        return list.Select(d => new DeliveryDto(
            d.Id, d.OrderId, d.Order.OrderNumber, d.Status,
            d.EstimatedPickupTime, d.EstimatedDeliveryTime,
            $"{d.Order.DeliveryAddress.Street} {d.Order.DeliveryAddress.Number}",
            d.Order.DeliveryAddress.City,
            d.CurrentLatitude, d.CurrentLongitude)).ToList();
    }

    public async Task PickupAsync(Guid deliveryId, CancellationToken ct = default)
    {
        var delivery = await deliveries.GetByIdAsync(deliveryId, ct)
            ?? throw new KeyNotFoundException($"Delivery {deliveryId} not found.");
        delivery.Status = DeliveryStatus.PickingUp;
        delivery.ActualPickupTime = DateTime.UtcNow;
        deliveries.Update(delivery);
        await deliveries.SaveChangesAsync(ct);
    }

    public async Task UpdateLocationAsync(Guid deliveryId, UpdateLocationRequest request, CancellationToken ct = default)
    {
        var delivery = await deliveries.GetByIdAsync(deliveryId, ct)
            ?? throw new KeyNotFoundException($"Delivery {deliveryId} not found.");
        delivery.CurrentLatitude = request.Latitude;
        delivery.CurrentLongitude = request.Longitude;
        if (delivery.Status == DeliveryStatus.PickingUp)
            delivery.Status = DeliveryStatus.InTransit;
        deliveries.Update(delivery);
        await deliveries.SaveChangesAsync(ct);
    }

    public async Task CompleteAsync(Guid deliveryId, CompleteDeliveryRequest request, CancellationToken ct = default)
    {
        var delivery = await deliveries.GetWithOrderAsync(deliveryId, ct)
            ?? throw new KeyNotFoundException($"Delivery {deliveryId} not found.");
        delivery.Status = DeliveryStatus.Delivered;
        delivery.ActualDeliveryTime = DateTime.UtcNow;
        delivery.ProofPhotoUrl = request.ProofPhotoUrl;
        delivery.ClientSignature = request.ClientSignature;
        delivery.ClientRating = request.ClientRating;
        delivery.ClientComment = request.ClientComment;

        delivery.Order.Status = OrderStatus.Delivered;
        delivery.Order.DeliveredAt = DateTime.UtcNow;

        deliveries.Update(delivery);
        orders.Update(delivery.Order);
        await deliveries.SaveChangesAsync(ct);
    }

    public async Task FailAsync(Guid deliveryId, FailDeliveryRequest request, CancellationToken ct = default)
    {
        var delivery = await deliveries.GetWithOrderAsync(deliveryId, ct)
            ?? throw new KeyNotFoundException($"Delivery {deliveryId} not found.");
        delivery.Status = DeliveryStatus.Failed;
        delivery.ClientComment = request.Reason;
        delivery.Order.Status = OrderStatus.Cancelled;
        delivery.Order.CancelledAt = DateTime.UtcNow;
        deliveries.Update(delivery);
        orders.Update(delivery.Order);
        await deliveries.SaveChangesAsync(ct);
    }
}
