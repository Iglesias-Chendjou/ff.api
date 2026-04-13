namespace FoodFirst.Dal.Entities;

public class DeliveryPerson
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ZoneId { get; set; }
    public string VehicleType { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
    public decimal? CurrentLatitude { get; set; }
    public decimal? CurrentLongitude { get; set; }
    public int TotalDeliveries { get; set; }
    public decimal AverageRating { get; set; }
    public string? BankAccountIBAN { get; set; }
    public bool IsVerified { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public Zone Zone { get; set; } = null!;
    public ICollection<Delivery> Deliveries { get; set; } = [];
}
