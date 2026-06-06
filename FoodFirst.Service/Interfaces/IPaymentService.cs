using FoodFirst.Dto.Payments;

namespace FoodFirst.Service.Interfaces;

public interface IPaymentService
{
    Task<PaymentIntentDto> CreateIntentAsync(Guid userId, Guid orderId, CancellationToken ct = default);
    Task<PaymentDto> ConfirmMockAsync(string paymentIntentId, CancellationToken ct = default);
    Task<PaymentDto?> GetByOrderAsync(Guid orderId, CancellationToken ct = default);
}
