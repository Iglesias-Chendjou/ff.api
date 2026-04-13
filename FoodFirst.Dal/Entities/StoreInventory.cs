using FoodFirst.Dal.Enums;

namespace FoodFirst.Dal.Entities;

public class StoreInventory
{
    public Guid Id { get; set; }
    public Guid StoreId { get; set; }
    public Guid ProductTemplateId { get; set; }
    public PriceRange SelectedRange { get; set; }
    public int Quantity { get; set; }
    public int AvailableQuantity { get; set; }
    public DateTime ExpirationDate { get; set; }
    public DateTime CheckedAt { get; set; }
    public Guid CheckedByUserId { get; set; }
    public bool IsPublished { get; set; }

    // Navigation properties
    public Store Store { get; set; } = null!;
    public ProductTemplate ProductTemplate { get; set; } = null!;
}
