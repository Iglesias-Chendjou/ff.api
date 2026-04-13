using FoodFirst.Dal.Enums;

namespace FoodFirst.Dal.Entities;

public class Payment
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public string? StripePaymentIntentId { get; set; }
    public string? StripeChargeId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "eur";
    public PaymentStatus Status { get; set; }
    public string? PaymentMethod { get; set; }
    public bool BancontactUsed { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    public DateTime? RefundedAt { get; set; }
    public decimal? RefundAmount { get; set; }

    // Navigation properties
    public Order Order { get; set; } = null!;
}
