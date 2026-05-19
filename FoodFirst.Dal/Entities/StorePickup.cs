namespace FoodFirst.Dal.Entities;

public class StorePickup
{
    public Guid Id { get; set; }
    public Guid CollectionRunId { get; set; }
    public Guid StoreId { get; set; }
    public int OrderInRun { get; set; }
    public DateTime? ArrivedAt { get; set; }
    public DateTime? PickedUpAt { get; set; }
    public decimal? TemperatureAtPickup { get; set; }
    public string? StoreSignatureUrl { get; set; }
    public string? PhotoUrl { get; set; }
    public string? Notes { get; set; }

    public CollectionRun CollectionRun { get; set; } = null!;
    public Store Store { get; set; } = null!;
    public ICollection<StorePickupItem> Items { get; set; } = [];
}
