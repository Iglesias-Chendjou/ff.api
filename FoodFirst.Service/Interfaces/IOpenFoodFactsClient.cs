using FoodFirst.Dto.Products;

namespace FoodFirst.Service.Interfaces;

public interface IOpenFoodFactsClient
{
    Task<OpenFoodFactsProductDto?> GetByBarcodeAsync(string barcode, CancellationToken ct = default);
    Task<OpenFoodFactsSearchResult> SearchAsync(string? brand, string? category, int page, int pageSize, CancellationToken ct = default);
}
