using FoodFirst.Dal.Context;
using FoodFirst.Dal.Entities;
using FoodFirst.Dal.Enums;
using FoodFirst.Dto.Payments;
using FoodFirst.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodFirst.Service.Implementations;

public class PaymentService(AppDbContext db, IOrderService orders) : IPaymentService
{
    public async Task<PaymentIntentDto> CreateIntentAsync(Guid userId, Guid orderId, CancellationToken ct = default)
    {
        var order = await db.Orders.FirstOrDefaultAsync(o => o.Id == orderId && o.ClientId == userId, ct)
            ?? throw new KeyNotFoundException($"Order {orderId} not found.");

        if (order.Status != OrderStatus.Pending)
            throw new InvalidOperationException($"Order {orderId} is not pending (status: {order.Status}).");

        var existing = await db.Payments.FirstOrDefaultAsync(p => p.OrderId == orderId, ct);
        if (existing is not null && existing.Status == PaymentStatus.Succeeded)
            throw new InvalidOperationException($"Order {orderId} is already paid.");

        var paymentIntentId = $"pi_mock_{orderId:N}";
        var clientSecret = $"{paymentIntentId}_secret_mock";

        if (existing is null)
        {
            existing = new Payment
            {
                Id = Guid.NewGuid(),
                OrderId = orderId,
                StripePaymentIntentId = paymentIntentId,
                Amount = order.TotalAmount,
                Currency = "eur",
                Status = PaymentStatus.Pending,
                CreatedAt = DateTime.UtcNow,
            };
            db.Payments.Add(existing);
        }
        else
        {
            existing.StripePaymentIntentId = paymentIntentId;
            existing.Amount = order.TotalAmount;
            existing.Status = PaymentStatus.Pending;
        }
        await db.SaveChangesAsync(ct);

        return new PaymentIntentDto(paymentIntentId, clientSecret, order.TotalAmount, "eur", IsMock: true);
    }

    public async Task<PaymentDto> ConfirmMockAsync(string paymentIntentId, CancellationToken ct = default)
    {
        var payment = await db.Payments.FirstOrDefaultAsync(p => p.StripePaymentIntentId == paymentIntentId, ct)
            ?? throw new KeyNotFoundException($"Payment {paymentIntentId} not found.");

        if (payment.Status == PaymentStatus.Succeeded) return Map(payment);

        payment.Status = PaymentStatus.Succeeded;
        payment.ConfirmedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);

        await orders.UpdateStatusAsync(payment.OrderId, OrderStatus.Paid, ct);

        return Map(payment);
    }

    public async Task<PaymentDto?> GetByOrderAsync(Guid orderId, CancellationToken ct = default)
    {
        var payment = await db.Payments.AsNoTracking().FirstOrDefaultAsync(p => p.OrderId == orderId, ct);
        return payment is null ? null : Map(payment);
    }

    private static PaymentDto Map(Payment p) => new(
        p.Id, p.OrderId, p.StripePaymentIntentId, p.Amount, p.Currency, p.Status, p.CreatedAt, p.ConfirmedAt);
}
