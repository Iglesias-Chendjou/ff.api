using FoodFirst.Dal.Enums;

namespace FoodFirst.Dal.Entities;

public class OrderItem
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid StoreInventoryId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public PriceRange PriceRange { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal { get; set; }
    public Guid StoreId { get; set; }

    // Navigation properties
    public Order Order { get; set; } = null!;
    public StoreInventory StoreInventory { get; set; } = null!;
    public Store Store { get; set; } = null!;
}
