namespace FoodFirst.Dal.Entities;

public class Zone
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string[] Communes { get; set; } = [];
    public int MaxDeliveryMinutes { get; set; }
    public bool IsActive { get; set; }

    // Navigation properties
    public ICollection<Store> Stores { get; set; } = [];
    public ICollection<DeliveryPerson> DeliveryPersons { get; set; } = [];
    public ICollection<Delivery> Deliveries { get; set; } = [];
}
