using FoodFirst.Dal.Enums;

namespace FoodFirst.Dto.Field;

// ─── Collection runs (collecteur) ──────────────────────────────────────

public record CollectionRunDto(
    Guid Id,
    Guid ZoneId,
    string ZoneName,
    DateTime ScheduledAt,
    DateTime? StartedAt,
    DateTime? CompletedAt,
    CollectionRunStatus Status,
    IReadOnlyList<StorePickupDto> Pickups);

public record StorePickupDto(
    Guid Id,
    Guid StoreId,
    string StoreName,
    string StoreAddress,
    decimal Latitude,
    decimal Longitude,
    int OrderInRun,
    DateTime? ArrivedAt,
    DateTime? PickedUpAt,
    decimal? TemperatureAtPickup,
    string? Notes,
    int ExpectedItemsCount);

public record CompletePickupRequest(
    decimal TemperatureAtPickup,
    string? PhotoUrl,
    string? StoreSignatureUrl,
    string? Notes,
    IReadOnlyList<PickupItemReportDto> Items);

public record PickupItemReportDto(
    Guid StorePickupItemId,
    int CollectedQuantity,
    bool IsConform,
    string? NonConformityReason);

// ─── Preparation (préparateur) ─────────────────────────────────────────

public record PreparationQueueItemDto(
    Guid OrderId,
    string OrderNumber,
    DateTime PaidAt,
    decimal SubTotal,
    int ItemsCount,
    Guid? PreparedByUserId,
    DateTime? PreparationStartedAt,
    DateTime? PreparedAt);
