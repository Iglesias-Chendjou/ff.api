using FoodFirst.Dal.Enums;

namespace FoodFirst.Dto.Stores;

public record StoreInventoryItemDto(
    Guid Id,
    Guid ProductTemplateId,
    string ProductName,
    PriceRange SelectedRange,
    int Quantity,
    int AvailableQuantity,
    DateTime ExpirationDate,
    bool IsPublished);

public record PublishInventoryRequest(IReadOnlyList<Guid> StoreInventoryIds);
