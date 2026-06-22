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

        // Selection du livreur dans la zone, parmi les livreurs en service et verifies
        // (mieux notes en premier). On retient le premier qui :
        //   - n'est PAS deja en tournee (un run InProgress n'accepte aucune nouvelle commande)
        //   - dont la tournee en preparation (run Pending) n'a pas atteint MaxStopsPerRun.
        var candidates = await db.DeliveryPersons
            .Where(dp => dp.ZoneId == order.ZoneId && dp.IsAvailable && dp.IsVerified)
            .OrderByDescending(dp => dp.AverageRating)
            .ToListAsync(ct);

        DeliveryPerson? driver = null;
        foreach (var dp in candidates)
        {
            // Pendant sa tournee, le livreur ne recoit pas de nouvelles commandes.
            var onActiveRun = await db.DeliveryRuns.AnyAsync(r =>
                r.DeliveryPersonUserId == dp.UserId &&
                r.Status == DeliveryRunStatus.InProgress, ct);
            if (onActiveRun) continue;

            // Capacite : nombre de stops non termines dans la tournee en preparation.
            var activeStops = await db.Deliveries.CountAsync(d =>
                d.DeliveryPersonId == dp.Id &&
                d.Status != DeliveryStatus.Delivered &&
                d.Status != DeliveryStatus.Failed &&
                d.Status != DeliveryStatus.Returned, ct);
            if (activeStops < BusinessRules.MaxStopsPerRun)
            {
                driver = dp;
                break;
            }
        }

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
        await db.SaveChangesAsync(ct);
        return delivery.Id;
    }
}
