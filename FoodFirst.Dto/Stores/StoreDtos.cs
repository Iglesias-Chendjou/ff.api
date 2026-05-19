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
    bool IsPublished,
    ListingReason Reason,
    UnsellableSubReason? UnsellableSubReason,
    string? ReasonNotes,
    int? DiscountPercentOverride);

public record PublishInventoryRequest(IReadOnlyList<Guid> StoreInventoryIds);

public record UpsertStoreInventoryItemDto(
    Guid? Id,
    Guid ProductTemplateId,
    PriceRange SelectedRange,
    int Quantity,
    DateTime ExpirationDate,
    ListingReason Reason,
    UnsellableSubReason? UnsellableSubReason,
    string? ReasonNotes,
    int? DiscountPercentOverride);

public record UpsertStoreInventoryRequest(IReadOnlyList<UpsertStoreInventoryItemDto> Items);
