using FoodFirst.Dal.Entities;
using FoodFirst.Dal.Enums;

namespace FoodFirst.Repository.Interfaces;

public interface IOrderRepository : IRepository<Order>
{
    Task<Order?> GetWithDetailsAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Order>> GetByClientAsync(Guid clientId, CancellationToken ct = default);
    Task<IReadOnlyList<Order>> GetByStatusAsync(OrderStatus status, CancellationToken ct = default);
}
