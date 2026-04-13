using FoodFirst.Dal.Enums;

namespace FoodFirst.Dal.Entities;

public class SurpriseBoxSubscription
{
    public Guid Id { get; set; }
    public Guid ClientId { get; set; }
    public Guid SurpriseBoxPlanId { get; set; }
    public string? StripeSubscriptionId { get; set; }
    public string? StripeCustomerId { get; set; }
    public SubscriptionStatus Status { get; set; }
    public Guid DeliveryAddressId { get; set; }
    public int DeliveriesUsedThisMonth { get; set; }
    public DateTime CurrentPeriodStart { get; set; }
    public DateTime CurrentPeriodEnd { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime NextBillingDate { get; set; }
    public DateTime? CancelledAt { get; set; }

    // Navigation properties
    public User Client { get; set; } = null!;
    public SurpriseBoxPlan Plan { get; set; } = null!;
    public Address DeliveryAddress { get; set; } = null!;
    public ICollection<Order> Orders { get; set; } = [];
}
