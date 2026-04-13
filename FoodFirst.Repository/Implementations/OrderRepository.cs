using FoodFirst.Dal.Context;
using FoodFirst.Dal.Entities;
using FoodFirst.Dal.Enums;
using FoodFirst.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodFirst.Repository.Implementations;

public class OrderRepository(AppDbContext db) : Repository<Order>(db), IOrderRepository
{
    public Task<Order?> GetWithDetailsAsync(Guid id, CancellationToken ct = default) =>
        Set.Include(o => o.Items)
            .Include(o => o.DeliveryAddress)
            .Include(o => o.Payment)
            .Include(o => o.Delivery)
            .FirstOrDefaultAsync(o => o.Id == id, ct);

    public async Task<IReadOnlyList<Order>> GetByClientAsync(Guid clientId, CancellationToken ct = default) =>
        await Set.AsNoTracking()
            .Where(o => o.ClientId == clientId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Order>> GetByStatusAsync(OrderStatus status, CancellationToken ct = default) =>
        await Set.AsNoTracking().Where(o => o.Status == status).ToListAsync(ct);
}
