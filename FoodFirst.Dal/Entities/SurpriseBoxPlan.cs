namespace FoodFirst.Dal.Entities;

public class SurpriseBoxPlan
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal MonthlyPrice { get; set; }
    public int DeliveriesPerMonth { get; set; }
    public decimal EstimatedBoxValue { get; set; }
    public bool IsActive { get; set; }

    // Navigation properties
    public ICollection<SurpriseBoxSubscription> Subscriptions { get; set; } = [];
}
