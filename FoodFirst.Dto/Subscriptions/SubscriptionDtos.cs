using FoodFirst.Dal.Enums;

namespace FoodFirst.Dto.Subscriptions;

public record CreateSubscriptionRequest(
    SubscriptionPlan Plan,
    Guid DeliveryAddressId,
    DayOfWeek PreferredDeliveryDay,
    string? PreferredCategories);

public record SubscriptionDto(
    Guid Id,
    SubscriptionPlan Plan,
    SubscriptionStatus Status,
    decimal MonthlyPrice,
    DateTime StartDate,
    DateTime NextBillingDate);
