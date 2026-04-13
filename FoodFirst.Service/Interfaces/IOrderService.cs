using FoodFirst.Dto.Orders;
using FoodFirst.Dal.Enums;

namespace FoodFirst.Service.Interfaces;

public interface IOrderService
{
    Task<CartValidationResponse> ValidateCartAsync(CartValidationRequest request, CancellationToken ct = default);
    Task<OrderDto> CreateAsync(Guid clientId, CreateOrderRequest request, CancellationToken ct = default);
    Task<OrderDto?> GetAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<OrderDto>> GetMyOrdersAsync(Guid clientId, CancellationToken ct = default);
    Task UpdateStatusAsync(Guid id, OrderStatus status, CancellationToken ct = default);
}
