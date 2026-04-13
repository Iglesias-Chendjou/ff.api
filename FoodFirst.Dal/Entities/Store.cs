namespace FoodFirst.Dal.Entities;

public class Store
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Commune { get; set; } = string.Empty;
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public Guid ZoneId { get; set; }
    public bool IsActive { get; set; }
    public DateTime ContractStartDate { get; set; }
    public string? TabletDeviceId { get; set; }

    // Navigation properties
    public Zone Zone { get; set; } = null!;
    public ICollection<StoreInventory> Inventories { get; set; } = [];
}
