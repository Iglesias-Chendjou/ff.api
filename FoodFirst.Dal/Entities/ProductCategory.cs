namespace FoodFirst.Dal.Entities;

public class ProductCategory
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }

    // Navigation properties
    public ICollection<ProductTemplate> ProductTemplates { get; set; } = [];
}
