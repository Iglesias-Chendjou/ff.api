using FoodFirst.Dal.Enums;

namespace FoodFirst.Dal.Entities;

public class CollectionRun
{
    public Guid Id { get; set; }
    public Guid CollectorUserId { get; set; }
    public Guid ZoneId { get; set; }
    public DateTime ScheduledAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public CollectionRunStatus Status { get; set; }
    public string? Notes { get; set; }

    public User Collector { get; set; } = null!;
    public Zone Zone { get; set; } = null!;
    public ICollection<StorePickup> Pickups { get; set; } = [];
}
