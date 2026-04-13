using FoodFirst.Dal.Enums;
using FoodFirst.Dto.Products;
using FoodFirst.Repository.Interfaces;
using FoodFirst.Service.Interfaces;

namespace FoodFirst.Service.Implementations;

public class ProductService(IStoreInventoryRepository inventories) : IProductService
{
    public async Task<IReadOnlyList<AvailableProductDto>> GetAvailableByZoneAsync(Guid zoneId, CancellationToken ct = default)
    {
        var items = await inventories.GetAvailableByZoneAsync(zoneId, ct);
        return items.Select(si =>
        {
            var original = si.SelectedRange switch
            {
                PriceRange.Low => si.ProductTemplate.PriceLowRange,
                PriceRange.Mid => si.ProductTemplate.PriceMidRange,
                PriceRange.High => si.ProductTemplate.PriceHighRange,
                _ => si.ProductTemplate.PriceMidRange
            };
            var discounted = Math.Round(original * (100 - si.ProductTemplate.DiscountPercent) / 100m, 2);
            return new AvailableProductDto(
                si.Id,
                si.ProductTemplateId,
                si.ProductTemplate.Name,
                si.ProductTemplate.Description,
                si.ProductTemplate.ImageUrl,
                si.ProductTemplate.Unit,
                si.ProductTemplate.Category.Name,
                si.SelectedRange,
                original,
                discounted,
                si.AvailableQuantity,
                si.ExpirationDate,
                si.StoreId,
                si.Store.Name);
        }).ToList();
    }
}
