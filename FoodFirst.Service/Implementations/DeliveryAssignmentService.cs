using FoodFirst.Dal.Context;
using FoodFirst.Dal.Entities;
using FoodFirst.Dal.Enums;
using FoodFirst.Service.Interfaces;
using FoodFirst.Tools.Helpers;
using Microsoft.EntityFrameworkCore;

namespace FoodFirst.Service.Implementations;

public class DeliveryAssignmentService(AppDbContext db) : IDeliveryAssignmentService
{
    public async Task<Guid?> AssignAsync(Guid orderId, CancellationToken ct = default)
    {
        var order = await db.Orders
            .Include(o => o.Delivery)
            .FirstOrDefaultAsync(o => o.Id == orderId, ct)
            ?? throw new KeyNotFoundException($"Order {orderId} not found.");

        if (order.Delivery is not null) return order.Delivery.Id;
        if (order.ZoneId is null)
            throw new InvalidOperationException("Order has no assigned zone.");

        var driver = await db.DeliveryPersons
            .Where(dp => dp.ZoneId == order.ZoneId && dp.IsAvailable && dp.IsVerified)
            .OrderByDescending(dp => dp.AverageRating)
            .FirstOrDefaultAsync(ct);

        if (driver is null) return null;

        var nowUtc = DateTime.UtcNow;
        var expectedDelivery = BusinessRules.ComputeExpectedDeliveryDate(nowUtc);
        var expectedPickup = expectedDelivery.AddHours(-2);

        var delivery = new Delivery
        {
            Id = Guid.NewGuid(),
            OrderId = order.Id,
            DeliveryPersonId = driver.Id,
            ZoneId = order.ZoneId.Value,
            Status = DeliveryStatus.Assigned,
            EstimatedPickupTime = expectedPickup,
            EstimatedDeliveryTime = expectedDelivery,
            CreatedAt = nowUtc
        };

        db.Deliveries.Add(delivery);
        driver.IsAvailable = false;
        await db.SaveChangesAsync(ct);
        return delivery.Id;
    }
}
