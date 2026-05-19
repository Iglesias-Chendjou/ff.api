using FoodFirst.Dal.Context;
using FoodFirst.Dal.Enums;
using FoodFirst.Dto.Field;
using FoodFirst.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodFirst.Service.Implementations;

public class PreparationService(AppDbContext db) : IPreparationService
{
    public async Task<IReadOnlyList<PreparationQueueItemDto>> GetQueueAsync(Guid preparerUserId, CancellationToken ct = default)
    {
        var rows = await db.Orders.AsNoTracking()
            .Where(o => (o.Status == OrderStatus.Paid || o.Status == OrderStatus.Preparing)
                        && (o.PreparedByUserId == null || o.PreparedByUserId == preparerUserId))
            .OrderBy(o => o.PaidAt)
            .Select(o => new PreparationQueueItemDto(
                o.Id, o.OrderNumber, o.PaidAt ?? o.CreatedAt, o.SubTotal,
                o.Items.Count, o.PreparedByUserId, o.PreparationStartedAt, o.PreparedAt))
            .ToListAsync(ct);
        return rows;
    }

    public async Task StartPreparingAsync(Guid orderId, Guid preparerUserId, CancellationToken ct = default)
    {
        var order = await db.Orders.FirstOrDefaultAsync(o => o.Id == orderId, ct)
            ?? throw new KeyNotFoundException($"Order {orderId} not found.");
        if (order.Status != OrderStatus.Paid)
            throw new InvalidOperationException($"Order must be Paid to start preparation (current: {order.Status}).");
        if (order.PreparedByUserId is not null && order.PreparedByUserId != preparerUserId)
            throw new InvalidOperationException("Order is already assigned to another preparer.");

        order.Status = OrderStatus.Preparing;
        order.PreparedByUserId = preparerUserId;
        order.PreparationStartedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
    }

    public async Task MarkReadyAsync(Guid orderId, Guid preparerUserId, CancellationToken ct = default)
    {
        var order = await db.Orders.FirstOrDefaultAsync(o => o.Id == orderId, ct)
            ?? throw new KeyNotFoundException($"Order {orderId} not found.");
        if (order.Status != OrderStatus.Preparing)
            throw new InvalidOperationException($"Order must be Preparing (current: {order.Status}).");
        if (order.PreparedByUserId != preparerUserId)
            throw new UnauthorizedAccessException("This order is assigned to another preparer.");

        order.Status = OrderStatus.ReadyForCollection;
        order.PreparedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
    }
}
