using FoodFirst.Dal.Enums;

namespace FoodFirst.Dal.Entities;

public class Subscription
{
    public Guid Id { get; set; }
    public Guid ClientId { get; set; }
    public SubscriptionPlan PlanType { get; set; }
    public string? StripeSubscriptionId { get; set; }
    public string? StripeCustomerId { get; set; }
    public SubscriptionStatus Status { get; set; }
    public decimal MonthlyPrice { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime NextBillingDate { get; set; }
    public DateTime? CancelledAt { get; set; }
    public Guid DeliveryAddressId { get; set; }
    public DayOfWeek PreferredDeliveryDay { get; set; }
    public string? PreferredCategories { get; set; }

    // Navigation properties
    public User Client { get; set; } = null!;
    public Address DeliveryAddress { get; set; } = null!;
}
