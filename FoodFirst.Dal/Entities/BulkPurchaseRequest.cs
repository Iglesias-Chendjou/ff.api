using FoodFirst.Dal.Enums;

namespace FoodFirst.Dal.Entities;

public class BulkPurchaseRequest
{
    public Guid Id { get; set; }
    public Guid SupplierId { get; set; }
    public string ProductDescription { get; set; } = string.Empty;
    public Guid? CategoryId { get; set; }
    public int Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public decimal? ProposedPricePerUnit { get; set; }
    public DateTime ExpirationDate { get; set; }
    public RequestStatus Status { get; set; }
    public string? AdminNotes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public Guid? ReviewedByUserId { get; set; }

    // Navigation properties
    public Supplier Supplier { get; set; } = null!;
    public ProductCategory? Category { get; set; }
}
