using FoodFirst.Dal.Enums;

namespace FoodFirst.Dto.Deliveries;

public record DeliveryDto(
    Guid Id,
    Guid OrderId,
    string OrderNumber,
    DeliveryStatus Status,
    DateTime EstimatedPickupTime,
    DateTime EstimatedDeliveryTime,
    string DeliveryStreet,
    string DeliveryCity,
    decimal? CurrentLatitude,
    decimal? CurrentLongitude);

public record UpdateLocationRequest(decimal Latitude, decimal Longitude);

public record CompleteDeliveryRequest(
    string? ProofPhotoUrl,
    string? ClientSignature,
    int? ClientRating,
    string? ClientComment,
    decimal? CollectedTemperature,
    decimal? DeliveredTemperature);

public record FailDeliveryRequest(string? Reason);
