using FoodFirst.Dal.Context;
using FoodFirst.Dal.Entities;
using FoodFirst.Dal.Enums;
using FoodFirst.Dto.Deliveries;
using FoodFirst.Service.Interfaces;
using FoodFirst.Tools.Helpers;
using Microsoft.EntityFrameworkCore;

namespace FoodFirst.Service.Implementations;

public class DeliveryService(AppDbContext db) : IDeliveryService
{
    public async Task<IReadOnlyList<DeliveryDto>> GetMineAsync(Guid deliveryPersonId, CancellationToken ct = default)
    {
        var list = await db.Deliveries.AsNoTracking()
            .Include(d => d.Order).ThenInclude(o => o.DeliveryAddress)
            .Where(d => d.DeliveryPersonId == deliveryPersonId)
            .OrderBy(d => d.EstimatedPickupTime)
            .ToListAsync(ct);
        return list.Select(MapDto).ToList();
    }

    public async Task<IReadOnlyList<DeliveryDto>> GetRouteAsync(Guid deliveryPersonId, CancellationToken ct = default)
    {
        var list = await db.Deliveries.AsNoTracking()
            .Include(d => d.Order).ThenInclude(o => o.DeliveryAddress)
            .Where(d => d.DeliveryPersonId == deliveryPersonId
                && (d.Status == DeliveryStatus.Assigned || d.Status == DeliveryStatus.PickingUp || d.Status == DeliveryStatus.InTransit))
            .ToListAsync(ct);

        var (wLat, wLon) = BusinessRules.WarehouseBrussels;
        return list
            .OrderBy(d => BusinessRules.HaversineKm(wLat, wLon, d.Order.DeliveryAddress.Latitude, d.Order.DeliveryAddress.Longitude))
            .Select(MapDto)
            .ToList();
    }

    public async Task PickupAsync(Guid deliveryId, CancellationToken ct = default)
    {
        var delivery = await db.Deliveries.FirstOrDefaultAsync(d => d.Id == deliveryId, ct)
            ?? throw new KeyNotFoundException($"Delivery {deliveryId} not found.");
        delivery.Status = DeliveryStatus.PickingUp;
        delivery.ActualPickupTime = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateLocationAsync(Guid deliveryId, UpdateLocationRequest request, CancellationToken ct = default)
    {
        var delivery = await db.Deliveries.FirstOrDefaultAsync(d => d.Id == deliveryId, ct)
            ?? throw new KeyNotFoundException($"Delivery {deliveryId} not found.");
        delivery.CurrentLatitude = request.Latitude;
        delivery.CurrentLongitude = request.Longitude;
        if (delivery.Status == DeliveryStatus.PickingUp)
            delivery.Status = DeliveryStatus.InTransit;
        await db.SaveChangesAsync(ct);
    }

    public async Task CompleteAsync(Guid deliveryId, CompleteDeliveryRequest request, CancellationToken ct = default)
    {
        var delivery = await db.Deliveries
            .Include(d => d.Order).ThenInclude(o => o.Items)
            .Include(d => d.DeliveryPerson)
            .FirstOrDefaultAsync(d => d.Id == deliveryId, ct)
            ?? throw new KeyNotFoundException($"Delivery {deliveryId} not found.");

        var nowUtc = DateTime.UtcNow;
        delivery.Status = DeliveryStatus.Delivered;
        delivery.ActualDeliveryTime = nowUtc;
        delivery.ProofPhotoUrl = request.ProofPhotoUrl;
        delivery.ClientSignature = request.ClientSignature;
        delivery.ClientRating = request.ClientRating;
        delivery.ClientComment = request.ClientComment;

        delivery.Order.Status = OrderStatus.Delivered;
        delivery.Order.DeliveredAt = nowUtc;

        delivery.DeliveryPerson.IsAvailable = true;
        delivery.DeliveryPerson.TotalDeliveries += 1;

        var isCompliant = request.DeliveredTemperature is null || request.DeliveredTemperature <= 7m;
        foreach (var item in delivery.Order.Items)
        {
            db.TraceabilityLogs.Add(new TraceabilityLog
            {
                Id = Guid.NewGuid(),
                OrderItemId = item.Id,
                StoreId = item.StoreId,
                ProductName = item.ProductName,
                ExpirationDate = nowUtc.AddDays(2),
                CollectedAt = delivery.ActualPickupTime ?? nowUtc,
                CollectedTemperature = request.CollectedTemperature,
                DeliveredAt = nowUtc,
                DeliveredTemperature = request.DeliveredTemperature,
                IsCompliant = isCompliant
            });
        }

        await db.SaveChangesAsync(ct);
    }

    public async Task FailAsync(Guid deliveryId, FailDeliveryRequest request, CancellationToken ct = default)
    {
        var delivery = await db.Deliveries
            .Include(d => d.Order)
            .Include(d => d.DeliveryPerson)
            .FirstOrDefaultAsync(d => d.Id == deliveryId, ct)
            ?? throw new KeyNotFoundException($"Delivery {deliveryId} not found.");
        delivery.Status = DeliveryStatus.Failed;
        delivery.ClientComment = request.Reason;
        delivery.Order.Status = OrderStatus.Cancelled;
        delivery.Order.CancelledAt = DateTime.UtcNow;
        delivery.DeliveryPerson.IsAvailable = true;
        await db.SaveChangesAsync(ct);
    }

    private static DeliveryDto MapDto(Delivery d) => new(
        d.Id, d.OrderId, d.Order.OrderNumber, d.Status,
        d.EstimatedPickupTime, d.EstimatedDeliveryTime,
        $"{d.Order.DeliveryAddress.Street} {d.Order.DeliveryAddress.Number}",
        d.Order.DeliveryAddress.City,
        d.CurrentLatitude, d.CurrentLongitude);
}
