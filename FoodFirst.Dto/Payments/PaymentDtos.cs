using FoodFirst.Dal.Enums;

namespace FoodFirst.Dto.Payments;

public record CreatePaymentIntentRequest(Guid OrderId);

public record PaymentIntentDto(
    string PaymentIntentId,
    string ClientSecret,
    decimal Amount,
    string Currency,
    bool IsMock);

public record PaymentDto(
    Guid Id,
    Guid OrderId,
    string? StripePaymentIntentId,
    decimal Amount,
    string Currency,
    PaymentStatus Status,
    DateTime CreatedAt,
    DateTime? ConfirmedAt);
