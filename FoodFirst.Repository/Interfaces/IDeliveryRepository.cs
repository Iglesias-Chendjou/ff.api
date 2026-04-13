using FoodFirst.Dal.Entities;

namespace FoodFirst.Repository.Interfaces;

public interface IDeliveryRepository : IRepository<Delivery>
{
    Task<IReadOnlyList<Delivery>> GetByDeliveryPersonAsync(Guid deliveryPersonId, CancellationToken ct = default);
    Task<Delivery?> GetWithOrderAsync(Guid id, CancellationToken ct = default);
}
