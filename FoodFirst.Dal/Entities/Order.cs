using FoodFirst.Dal.Enums;

namespace FoodFirst.Dal.Entities;

public class Order
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public Guid ClientId { get; set; }
    public Guid DeliveryAddressId { get; set; }
    public Guid? ZoneId { get; set; }
    public OrderStatus Status { get; set; }
    public decimal SubTotal { get; set; }
    public decimal DeliveryFee { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TVA { get; set; }
    public string? StripePaymentIntentId { get; set; }
    public string? StripeChargeId { get; set; }
    public bool IsSubscriptionOrder { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? PaidAt { get; set; }
    public DateTime? PreparedAt { get; set; }
    public DateTime? CollectedAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public DateTime? CancelledAt { get; set; }

    // Navigation properties
    public User Client { get; set; } = null!;
    public Address DeliveryAddress { get; set; } = null!;
    public Zone? Zone { get; set; }
    public ICollection<OrderItem> Items { get; set; } = [];
    public Payment? Payment { get; set; }
    public Delivery? Delivery { get; set; }
}
