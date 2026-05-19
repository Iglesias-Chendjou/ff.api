using FoodFirst.Dal.Enums;

namespace FoodFirst.Dal.Entities;

public class Membership
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public MembershipStatus Status { get; set; }
    public decimal MonthlyPrice { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? NextBillingDate { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? StripeSubscriptionId { get; set; }
    public string? StripeCustomerId { get; set; }

    public User User { get; set; } = null!;
}
