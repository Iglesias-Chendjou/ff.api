using FoodFirst.Dal.Enums;

namespace FoodFirst.Dal.Entities;

public class Delivery
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid DeliveryPersonId { get; set; }
    public Guid ZoneId { get; set; }
    public DeliveryStatus Status { get; set; }
    public DateTime EstimatedPickupTime { get; set; }
    public DateTime? ActualPickupTime { get; set; }
    public DateTime EstimatedDeliveryTime { get; set; }
    public DateTime? ActualDeliveryTime { get; set; }
    public decimal? CurrentLatitude { get; set; }
    public decimal? CurrentLongitude { get; set; }
    public string? ProofPhotoUrl { get; set; }
    public string? ClientSignature { get; set; }
    public int? ClientRating { get; set; }
    public string? ClientComment { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public Order Order { get; set; } = null!;
    public DeliveryPerson DeliveryPerson { get; set; } = null!;
    public Zone Zone { get; set; } = null!;
}
