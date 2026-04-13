using FoodFirst.Dal.Enums;

namespace FoodFirst.Dal.Entities;

public class Supplier
{
    public Guid Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string ContactName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public SupplierType SupplierType { get; set; }
    public string VATNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsApproved { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }

    // Navigation properties
    public ICollection<BulkPurchaseRequest> BulkPurchaseRequests { get; set; } = [];
}
