using FoodFirst.Dal.Context;
using FoodFirst.Dal.Entities;
using FoodFirst.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodFirst.Repository.Implementations;

public class StoreInventoryRepository(AppDbContext db) : Repository<StoreInventory>(db), IStoreInventoryRepository
{
    public async Task<IReadOnlyList<StoreInventory>> GetAvailableByZoneAsync(Guid zoneId, CancellationToken ct = default) =>
        await Set.AsNoTracking()
            .Include(si => si.ProductTemplate).ThenInclude(pt => pt.Category)
            .Include(si => si.Store)
            .Where(si => si.IsPublished
                && si.AvailableQuantity > 0
                && si.ExpirationDate > DateTime.UtcNow
                && si.Store.ZoneId == zoneId
                && si.Store.IsActive)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<StoreInventory>> GetByStoreAsync(Guid storeId, CancellationToken ct = default) =>
        await Set.AsNoTracking()
            .Include(si => si.ProductTemplate)
            .Where(si => si.StoreId == storeId)
            .ToListAsync(ct);
}
