using FoodFirst.Dal.Context;
using FoodFirst.Dal.Enums;
using FoodFirst.Dto.Field;
using FoodFirst.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodFirst.Service.Implementations;

public class CollectionRunService(AppDbContext db) : ICollectionRunService
{
    public async Task<IReadOnlyList<CollectionRunDto>> GetMyRunsAsync(Guid collectorUserId, CancellationToken ct = default)
    {
        var runs = await db.CollectionRuns.AsNoTracking()
            .Include(r => r.Zone)
            .Include(r => r.Pickups).ThenInclude(p => p.Store)
            .Include(r => r.Pickups).ThenInclude(p => p.Items)
            .Where(r => r.CollectorUserId == collectorUserId
                        && r.Status != CollectionRunStatus.Cancelled
                        && r.Status != CollectionRunStatus.Completed)
            .OrderBy(r => r.ScheduledAt)
            .ToListAsync(ct);

        return runs.Select(Map).ToList();
    }

    public async Task<CollectionRunDto?> GetByIdAsync(Guid runId, Guid collectorUserId, CancellationToken ct = default)
    {
        var run = await db.CollectionRuns.AsNoTracking()
            .Include(r => r.Zone)
            .Include(r => r.Pickups).ThenInclude(p => p.Store)
            .Include(r => r.Pickups).ThenInclude(p => p.Items)
            .FirstOrDefaultAsync(r => r.Id == runId && r.CollectorUserId == collectorUserId, ct);
        return run is null ? null : Map(run);
    }

    public async Task StartRunAsync(Guid runId, Guid collectorUserId, CancellationToken ct = default)
    {
        var run = await db.CollectionRuns
            .FirstOrDefaultAsync(r => r.Id == runId && r.CollectorUserId == collectorUserId, ct)
            ?? throw new KeyNotFoundException($"Run {runId} not found.");
        if (run.Status != CollectionRunStatus.Pending)
            throw new InvalidOperationException($"Run already {run.Status}.");
        run.Status = CollectionRunStatus.InProgress;
        run.StartedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
    }

    public async Task CompleteRunAsync(Guid runId, Guid collectorUserId, CancellationToken ct = default)
    {
        var run = await db.CollectionRuns
            .Include(r => r.Pickups)
            .FirstOrDefaultAsync(r => r.Id == runId && r.CollectorUserId == collectorUserId, ct)
            ?? throw new KeyNotFoundException($"Run {runId} not found.");
        if (run.Status != CollectionRunStatus.InProgress)
            throw new InvalidOperationException($"Run is not in progress (current: {run.Status}).");
        if (run.Pickups.Any(p => p.PickedUpAt is null))
            throw new InvalidOperationException("All pickups must be completed before closing the run.");
        run.Status = CollectionRunStatus.Completed;
        run.CompletedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
    }

    public async Task CompletePickupAsync(Guid pickupId, Guid collectorUserId, CompletePickupRequest request, CancellationToken ct = default)
    {
        var pickup = await db.StorePickups
            .Include(p => p.CollectionRun)
            .Include(p => p.Items)
            .FirstOrDefaultAsync(p => p.Id == pickupId, ct)
            ?? throw new KeyNotFoundException($"Pickup {pickupId} not found.");

        if (pickup.CollectionRun.CollectorUserId != collectorUserId)
            throw new UnauthorizedAccessException("This pickup belongs to another collector.");

        if (pickup.CollectionRun.Status != CollectionRunStatus.InProgress)
            throw new InvalidOperationException($"Run is not in progress (current: {pickup.CollectionRun.Status}).");

        var now = DateTime.UtcNow;
        if (pickup.ArrivedAt is null) pickup.ArrivedAt = now;
        pickup.PickedUpAt = now;
        pickup.TemperatureAtPickup = request.TemperatureAtPickup;
        pickup.PhotoUrl = request.PhotoUrl;
        pickup.StoreSignatureUrl = request.StoreSignatureUrl;
        pickup.Notes = request.Notes;

        var itemsById = pickup.Items.ToDictionary(i => i.Id);
        foreach (var report in request.Items)
        {
            if (!itemsById.TryGetValue(report.StorePickupItemId, out var item)) continue;
            item.CollectedQuantity = report.CollectedQuantity;
            item.IsConform = report.IsConform;
            item.NonConformityReason = report.NonConformityReason;
        }

        await db.SaveChangesAsync(ct);
    }

    private static CollectionRunDto Map(Dal.Entities.CollectionRun r) => new(
        r.Id, r.ZoneId, r.Zone.Name, r.ScheduledAt, r.StartedAt, r.CompletedAt, r.Status,
        r.Pickups
            .OrderBy(p => p.OrderInRun)
            .Select(p => new StorePickupDto(
                p.Id, p.StoreId, p.Store.Name, p.Store.Address, p.Store.Latitude, p.Store.Longitude,
                p.OrderInRun, p.ArrivedAt, p.PickedUpAt, p.TemperatureAtPickup, p.Notes,
                p.Items.Count))
            .ToList());
}
