using FoodFirst.Dal.Context;
using FoodFirst.Dal.Enums;
using FoodFirst.Dto.Field;
using FoodFirst.Service.Interfaces;
using FoodFirst.Tools.Helpers;
using Microsoft.EntityFrameworkCore;

namespace FoodFirst.Service.Implementations;

public class DeliveryRunService(AppDbContext db, IDeliveryAssignmentService assignment) : IDeliveryRunService
{
    public async Task<IReadOnlyList<DeliveryRunDto>> GetMyRunsAsync(Guid userId, CancellationToken ct = default)
    {
        var runs = await db.DeliveryRuns.AsNoTracking()
            .Include(r => r.Zone)
            .Include(r => r.Deliveries).ThenInclude(d => d.Order).ThenInclude(o => o.DeliveryAddress)
            .Where(r => r.DeliveryPersonUserId == userId
                && r.Status != DeliveryRunStatus.Cancelled
                && r.Status != DeliveryRunStatus.Completed)
            .OrderBy(r => r.ScheduledAt)
            .ToListAsync(ct);
        return runs.Select(Map).ToList();
    }

    public async Task<DeliveryRunDto?> GetByIdAsync(Guid runId, Guid userId, CancellationToken ct = default)
    {
        var run = await db.DeliveryRuns.AsNoTracking()
            .Include(r => r.Zone)
            .Include(r => r.Deliveries).ThenInclude(d => d.Order).ThenInclude(o => o.DeliveryAddress)
            .FirstOrDefaultAsync(r => r.Id == runId && r.DeliveryPersonUserId == userId, ct);
        return run is null ? null : Map(run);
    }

    public async Task<DeliveryRunDto?> GetByCodeAsync(string code, Guid userId, CancellationToken ct = default)
    {
        var trimmed = code.Trim();
        var run = await db.DeliveryRuns.AsNoTracking()
            .Include(r => r.Zone)
            .Include(r => r.Deliveries).ThenInclude(d => d.Order).ThenInclude(o => o.DeliveryAddress)
            .FirstOrDefaultAsync(r => r.Code == trimmed && r.DeliveryPersonUserId == userId, ct);
        return run is null ? null : Map(run);
    }

    public async Task StartRunAsync(Guid runId, Guid userId, CancellationToken ct = default)
    {
        var run = await db.DeliveryRuns
            .FirstOrDefaultAsync(r => r.Id == runId && r.DeliveryPersonUserId == userId, ct)
            ?? throw new KeyNotFoundException($"DeliveryRun {runId} not found.");
        if (run.Status != DeliveryRunStatus.Pending)
            throw new InvalidOperationException($"Run already {run.Status}.");
        run.Status = DeliveryRunStatus.InProgress;
        run.StartedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
    }

    public async Task EnsureRunForReadyOrderAsync(Guid orderId, CancellationToken ct = default)
    {
        var delivery = await db.Deliveries
            .Include(d => d.DeliveryPerson)
            .Include(d => d.Order).ThenInclude(o => o.DeliveryAddress)
            .FirstOrDefaultAsync(d => d.OrderId == orderId, ct);

        if (delivery is null) return;
        if (delivery.DeliveryRunId is not null)
        {
            await ReorderRunByDistanceAsync(delivery.DeliveryRunId.Value, ct);
            return;
        }

        var driverUserId = delivery.DeliveryPerson.UserId;
        var zoneId = delivery.ZoneId;

        // Timeline FoodFirst :
        //   - commande < 17h  -> livraison aujourd'hui matin
        //   - commande >= 17h -> livraison demain matin
        // La preparation se fait le soir J, la tournee part a 6h locale J+1.
        var (deliveryDateLocal, scheduledAtUtc) = BusinessRules.GetNextDeliveryRunWindow(DateTime.UtcNow);

        var run = await db.DeliveryRuns
            .FirstOrDefaultAsync(r => r.DeliveryPersonUserId == driverUserId
                && r.ZoneId == zoneId
                && r.Status == DeliveryRunStatus.Pending
                && r.ScheduledAt == scheduledAtUtc, ct);

        if (run is null)
        {
            run = new Dal.Entities.DeliveryRun
            {
                Id = Guid.NewGuid(),
                Code = $"DR-{deliveryDateLocal:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}",
                DeliveryPersonUserId = driverUserId,
                ZoneId = zoneId,
                ScheduledAt = scheduledAtUtc,
                Status = DeliveryRunStatus.Pending,
                Notes = $"Tournee du {deliveryDateLocal:dd/MM/yyyy} (auto-generee a la preparation)"
            };
            db.DeliveryRuns.Add(run);
        }

        delivery.DeliveryRunId = run.Id;
        await db.SaveChangesAsync(ct);

        await ReorderRunByDistanceAsync(run.Id, ct);
    }

    private async Task ReorderRunByDistanceAsync(Guid runId, CancellationToken ct)
    {
        var deliveries = await db.Deliveries
            .Include(d => d.Order).ThenInclude(o => o.DeliveryAddress)
            .Where(d => d.DeliveryRunId == runId)
            .ToListAsync(ct);

        var (wLat, wLon) = BusinessRules.WarehouseBrussels;
        var ordered = deliveries
            .OrderBy(d => BusinessRules.HaversineKm(wLat, wLon,
                d.Order.DeliveryAddress.Latitude,
                d.Order.DeliveryAddress.Longitude))
            .ToList();

        var rank = 1;
        foreach (var d in ordered)
        {
            d.OrderInRun = rank++;
        }
        await db.SaveChangesAsync(ct);
    }

    public async Task CompleteRunAsync(Guid runId, Guid userId, CancellationToken ct = default)
    {
        var run = await db.DeliveryRuns
            .Include(r => r.Deliveries)
            .FirstOrDefaultAsync(r => r.Id == runId && r.DeliveryPersonUserId == userId, ct)
            ?? throw new KeyNotFoundException($"DeliveryRun {runId} not found.");
        if (run.Status != DeliveryRunStatus.InProgress)
            throw new InvalidOperationException($"Run is not in progress (current: {run.Status}).");

        // Toutes les livraisons doivent etre soit Delivered soit Failed soit Returned
        var unfinished = run.Deliveries
            .Any(d => d.Status != DeliveryStatus.Delivered
                   && d.Status != DeliveryStatus.Failed
                   && d.Status != DeliveryStatus.Returned);
        if (unfinished)
            throw new InvalidOperationException("All deliveries must be terminated (Delivered/Failed/Returned).");

        run.Status = DeliveryRunStatus.Completed;
        run.CompletedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);

        // Reassignation de fin de tournee : les commandes de la zone restees sans
        // livreur (aucun disponible au moment du paiement, car en tournee) sont
        // reprises maintenant que le livreur est libere -> prochaine tournee.
        await ReassignOrphanOrdersAsync(run.ZoneId, ct);
    }

    private async Task ReassignOrphanOrdersAsync(Guid zoneId, CancellationToken ct)
    {
        var orphans = await db.Orders
            .Where(o => o.ZoneId == zoneId
                && o.Delivery == null
                && (o.Status == OrderStatus.Paid
                    || o.Status == OrderStatus.Preparing
                    || o.Status == OrderStatus.ReadyForCollection))
            .OrderBy(o => o.CreatedAt)
            .Select(o => new { o.Id, o.Status })
            .ToListAsync(ct);

        foreach (var o in orphans)
        {
            var deliveryId = await assignment.AssignAsync(o.Id, ct);
            // Si la commande etait deja prete, on l'attache tout de suite a la tournee.
            if (deliveryId is not null && o.Status == OrderStatus.ReadyForCollection)
                await EnsureRunForReadyOrderAsync(o.Id, ct);
        }
    }

    private static DeliveryRunDto Map(Dal.Entities.DeliveryRun r)
    {
        var stops = r.Deliveries
            .OrderBy(d => d.OrderInRun)
            .Select(d => new DeliveryRunStopDto(
                d.Id, d.OrderId, d.Order.OrderNumber, d.Status, d.OrderInRun,
                $"{d.Order.DeliveryAddress.Street} {d.Order.DeliveryAddress.Number}",
                d.Order.DeliveryAddress.City,
                d.Order.DeliveryAddress.Latitude,
                d.Order.DeliveryAddress.Longitude,
                d.EstimatedDeliveryTime))
            .ToList();

        return new DeliveryRunDto(
            r.Id, r.Code, r.ZoneId, r.Zone.Name,
            r.ScheduledAt, r.StartedAt, r.CompletedAt, r.Status, stops);
    }
}
