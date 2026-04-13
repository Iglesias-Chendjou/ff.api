using FoodFirst.Dto.Stores;
using FoodFirst.Repository.Interfaces;
using FoodFirst.Service.Interfaces;

namespace FoodFirst.Service.Implementations;

public class StoreService(IStoreInventoryRepository inventories) : IStoreService
{
    public async Task<IReadOnlyList<StoreInventoryItemDto>> GetCatalogAsync(Guid storeId, CancellationToken ct = default)
    {
        var items = await inventories.GetByStoreAsync(storeId, ct);
        return items.Select(si => new StoreInventoryItemDto(
            si.Id, si.ProductTemplateId, si.ProductTemplate.Name,
            si.SelectedRange, si.Quantity, si.AvailableQuantity, si.ExpirationDate, si.IsPublished)).ToList();
    }

    public async Task PublishInventoryAsync(Guid storeId, PublishInventoryRequest request, CancellationToken ct = default)
    {
        foreach (var id in request.StoreInventoryIds)
        {
            var inv = await inventories.GetByIdAsync(id, ct);
            if (inv is null || inv.StoreId != storeId) continue;
            inv.IsPublished = true;
            inventories.Update(inv);
        }
        await inventories.SaveChangesAsync(ct);
    }
}
