namespace FoodFirst.Dal.Entities;

public class ProductTemplate
{
    public Guid Id { get; set; }
    public Guid CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string Unit { get; set; } = string.Empty;
    public decimal PriceLowRange { get; set; }
    public decimal PriceMidRange { get; set; }
    public decimal PriceHighRange { get; set; }
    public int DiscountPercent { get; set; } = 50;
    public bool IsActive { get; set; }

    // Navigation properties
    public ProductCategory Category { get; set; } = null!;
    public ICollection<StoreInventory> StoreInventories { get; set; } = [];
}
