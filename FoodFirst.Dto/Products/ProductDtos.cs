using FoodFirst.Dal.Enums;

namespace FoodFirst.Dto.Products;

public record AvailableProductDto(
    Guid StoreInventoryId,
    Guid ProductTemplateId,
    string Name,
    string? Description,
    string? ImageUrl,
    string Unit,
    string CategoryName,
    PriceRange PriceRange,
    decimal OriginalPrice,
    decimal DiscountedPrice,
    int AvailableQuantity,
    DateTime ExpirationDate,
    Guid StoreId,
    string StoreName);
