using FoodFirst.Dal.Context;
using FoodFirst.Dal.Entities;
using FoodFirst.Dto.Stores;
using FoodFirst.Repository.Interfaces;
using FoodFirst.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodFirst.Service.Implementations;

public class StoreService(AppDbContext db, IStoreInventoryRepository inventories) : IStoreService
{
    public async Task<IReadOnlyList<StoreInventoryItemDto>> GetCatalogAsync(Guid storeId, CancellationToken ct = default)
    {
        var items = await inventories.GetByStoreAsync(storeId, ct);
        return items.Select(Map).ToList();
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

    public async Task<IReadOnlyList<StoreInventoryItemDto>> UpsertInventoryAsync(
        Guid storeId,
        Guid checkedByUserId,
        UpsertStoreInventoryRequest request,
        CancellationToken ct = default)
    {
        var ids = request.Items.Where(i => i.Id.HasValue).Select(i => i.Id!.Value).ToArray();
        var existingById = ids.Length == 0
            ? new Dictionary<Guid, StoreInventory>()
            : await db.StoreInventories
                .Where(si => ids.Contains(si.Id) && si.StoreId == storeId)
                .ToDictionaryAsync(si => si.Id, ct);

        var now = DateTime.UtcNow;
        var touched = new List<StoreInventory>();

        foreach (var item in request.Items)
        {
            StoreInventory inv;
            if (item.Id.HasValue && existingById.TryGetValue(item.Id.Value, out var existing))
            {
                inv = existing;
            }
            else
            {
                inv = new StoreInventory
                {
                    Id = Guid.NewGuid(),
                    StoreId = storeId,
                    AvailableQuantity = item.Quantity,
                    IsPublished = false
                };
                db.StoreInventories.Add(inv);
            }

            inv.ProductTemplateId = item.ProductTemplateId;
            inv.SelectedRange = item.SelectedRange;
            inv.Quantity = item.Quantity;
            inv.AvailableQuantity = item.Quantity;
            inv.ExpirationDate = item.ExpirationDate;
            inv.Reason = item.Reason;
            inv.UnsellableSubReason = item.UnsellableSubReason;
            inv.ReasonNotes = item.ReasonNotes;
            inv.DiscountPercentOverride = item.DiscountPercentOverride;
            inv.CheckedAt = now;
            inv.CheckedByUserId = checkedByUserId;
            touched.Add(inv);
        }

        await db.SaveChangesAsync(ct);

        var ptIds = touched.Select(t => t.ProductTemplateId).Distinct().ToArray();
        var templates = await db.ProductTemplates
            .Where(pt => ptIds.Contains(pt.Id))
            .ToDictionaryAsync(pt => pt.Id, ct);

        return touched
            .Select(t => Map(t, templates.TryGetValue(t.ProductTemplateId, out var pt) ? pt.Name : ""))
            .ToList();
    }

    private static StoreInventoryItemDto Map(StoreInventory si) =>
        Map(si, si.ProductTemplate?.Name ?? "");

    private static StoreInventoryItemDto Map(StoreInventory si, string productName) => new(
        si.Id, si.ProductTemplateId, productName,
        si.SelectedRange, si.Quantity, si.AvailableQuantity, si.ExpirationDate, si.IsPublished,
        si.Reason, si.UnsellableSubReason, si.ReasonNotes, si.DiscountPercentOverride);
}
