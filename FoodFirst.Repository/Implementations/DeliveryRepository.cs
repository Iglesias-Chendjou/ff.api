using FoodFirst.Dal.Context;
using FoodFirst.Dal.Entities;
using FoodFirst.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodFirst.Repository.Implementations;

public class DeliveryRepository(AppDbContext db) : Repository<Delivery>(db), IDeliveryRepository
{
    public async Task<IReadOnlyList<Delivery>> GetByDeliveryPersonAsync(Guid deliveryPersonId, CancellationToken ct = default) =>
        await Set.AsNoTracking()
            .Include(d => d.Order).ThenInclude(o => o.DeliveryAddress)
            .Where(d => d.DeliveryPersonId == deliveryPersonId)
            .OrderBy(d => d.EstimatedPickupTime)
            .ToListAsync(ct);

    public Task<Delivery?> GetWithOrderAsync(Guid id, CancellationToken ct = default) =>
        Set.Include(d => d.Order).ThenInclude(o => o.Items)
           .Include(d => d.Order).ThenInclude(o => o.DeliveryAddress)
           .FirstOrDefaultAsync(d => d.Id == id, ct);
}
