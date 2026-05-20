using FoodFirst.Dal.Enums;

namespace FoodFirst.Dal.Entities;

public class DeliveryRun
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public Guid DeliveryPersonUserId { get; set; }
    public Guid ZoneId { get; set; }
    public DateTime ScheduledAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DeliveryRunStatus Status { get; set; }
    public string? Notes { get; set; }

    public User DeliveryPersonUser { get; set; } = null!;
    public Zone Zone { get; set; } = null!;
    public ICollection<Delivery> Deliveries { get; set; } = [];
}
