namespace FoodFirst.Dal.Entities;

public class TraceabilityLog
{
    public Guid Id { get; set; }
    public Guid OrderItemId { get; set; }
    public Guid StoreId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? BatchNumber { get; set; }
    public DateTime ExpirationDate { get; set; }
    public DateTime CollectedAt { get; set; }
    public decimal? CollectedTemperature { get; set; }
    public DateTime DeliveredAt { get; set; }
    public decimal? DeliveredTemperature { get; set; }
    public bool IsCompliant { get; set; }

    // Navigation properties
    public OrderItem OrderItem { get; set; } = null!;
    public Store Store { get; set; } = null!;
}
