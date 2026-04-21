using FoodFirst.Dal.Entities;

namespace FoodFirst.Repository.Interfaces;

public interface IStoreInventoryRepository : IRepository<StoreInventory>
{
    Task<IReadOnlyList<StoreInventory>> GetAvailableByZoneAsync(Guid zoneId, CancellationToken ct = default);
    Task<IReadOnlyList<StoreInventory>> GetAllAvailableAsync(CancellationToken ct = default);
    Task<IReadOnlyList<StoreInventory>> GetByStoreAsync(Guid storeId, CancellationToken ct = default);
}
