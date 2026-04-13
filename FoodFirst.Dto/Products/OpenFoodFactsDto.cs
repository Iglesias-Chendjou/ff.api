namespace FoodFirst.Dto.Products;

public record OpenFoodFactsProductDto(
    string Barcode,
    string? Name,
    string? Brand,
    string? ImageUrl,
    string? Quantity,
    IReadOnlyList<string> Categories,
    string? NutritionGrade);

public record OpenFoodFactsSearchResult(
    int Count,
    int Page,
    int PageSize,
    IReadOnlyList<OpenFoodFactsProductDto> Products);
