using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using FoodFirst.Dto.Products;
using FoodFirst.Service.Interfaces;

namespace FoodFirst.Service.Implementations;

public class OpenFoodFactsClient(HttpClient http) : IOpenFoodFactsClient
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString
    };

    public async Task<OpenFoodFactsProductDto?> GetByBarcodeAsync(string barcode, CancellationToken ct = default)
    {
        var response = await http.GetAsync($"api/v2/product/{Uri.EscapeDataString(barcode)}.json", ct);
        if (!response.IsSuccessStatusCode) return null;

        var payload = await response.Content.ReadFromJsonAsync<OffEnvelope>(JsonOptions, ct);
        if (payload is null || payload.Status != 1 || payload.Product is null) return null;

        var p = payload.Product;
        return new OpenFoodFactsProductDto(
            barcode,
            p.ProductName,
            p.Brands,
            p.ImageFrontUrl ?? p.ImageUrl,
            p.Quantity,
            p.CategoriesTags ?? [],
            p.NutritionGrades);
    }

    public async Task<OpenFoodFactsSearchResult> SearchAsync(string? brand, string? category, int page, int pageSize, CancellationToken ct = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = new List<string>
        {
            $"page={page}",
            $"page_size={pageSize}",
            "fields=code,product_name,brands,image_front_url,image_url,quantity,categories_tags_en,nutrition_grades"
        };
        if (!string.IsNullOrWhiteSpace(brand)) query.Add($"brands_tags={Uri.EscapeDataString(brand)}");
        if (!string.IsNullOrWhiteSpace(category)) query.Add($"categories_tags_en={Uri.EscapeDataString(category)}");

        var url = $"api/v2/search?{string.Join('&', query)}";
        var response = await http.GetAsync(url, ct);
        if (!response.IsSuccessStatusCode)
            return new OpenFoodFactsSearchResult(0, page, pageSize, []);

        var payload = await response.Content.ReadFromJsonAsync<OffSearchEnvelope>(JsonOptions, ct);
        if (payload is null)
            return new OpenFoodFactsSearchResult(0, page, pageSize, []);

        var products = (payload.Products ?? [])
            .Select(p => new OpenFoodFactsProductDto(
                p.Code ?? string.Empty,
                p.ProductName,
                p.Brands,
                p.ImageFrontUrl ?? p.ImageUrl,
                p.Quantity,
                p.CategoriesTags ?? [],
                p.NutritionGrades))
            .ToList();

        return new OpenFoodFactsSearchResult(payload.Count, page, pageSize, products);
    }

    private sealed class OffEnvelope
    {
        [JsonPropertyName("status")] public int Status { get; set; }
        [JsonPropertyName("product")] public OffProduct? Product { get; set; }
    }

    private sealed class OffSearchEnvelope
    {
        [JsonPropertyName("count")] public int Count { get; set; }
        [JsonPropertyName("page")] public int Page { get; set; }
        [JsonPropertyName("page_size")] public int PageSize { get; set; }
        [JsonPropertyName("products")] public OffProduct[]? Products { get; set; }
    }

    private sealed class OffProduct
    {
        [JsonPropertyName("code")] public string? Code { get; set; }
        [JsonPropertyName("product_name")] public string? ProductName { get; set; }
        [JsonPropertyName("brands")] public string? Brands { get; set; }
        [JsonPropertyName("image_front_url")] public string? ImageFrontUrl { get; set; }
        [JsonPropertyName("image_url")] public string? ImageUrl { get; set; }
        [JsonPropertyName("quantity")] public string? Quantity { get; set; }
        [JsonPropertyName("categories_tags_en")] public string[]? CategoriesTags { get; set; }
        [JsonPropertyName("nutrition_grades")] public string? NutritionGrades { get; set; }
    }
}
