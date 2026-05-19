using FoodFirst.Dal.Enums;

namespace FoodFirst.Dto.Memberships;

public record MembershipDto(
    Guid Id,
    MembershipStatus Status,
    decimal MonthlyPrice,
    DateTime StartDate,
    DateTime? NextBillingDate,
    DateTime? CancelledAt);
