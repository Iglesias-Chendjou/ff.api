using FoodFirst.Dto.Stores;

namespace FoodFirst.Service.Interfaces;

public interface IStoreService
{
    Task<IReadOnlyList<StoreInventoryItemDto>> GetCatalogAsync(Guid storeId, CancellationToken ct = default);
    Task PublishInventoryAsync(Guid storeId, PublishInventoryRequest request, CancellationToken ct = default);
}
