using FoodFirst.Dto.Field;

namespace FoodFirst.Service.Interfaces;

public interface IDeliveryRunService
{
    Task<IReadOnlyList<DeliveryRunDto>> GetMyRunsAsync(Guid userId, CancellationToken ct = default);
    Task<DeliveryRunDto?> GetByIdAsync(Guid runId, Guid userId, CancellationToken ct = default);
    Task<DeliveryRunDto?> GetByCodeAsync(string code, Guid userId, CancellationToken ct = default);
    Task StartRunAsync(Guid runId, Guid userId, CancellationToken ct = default);
    Task CompleteRunAsync(Guid runId, Guid userId, CancellationToken ct = default);
    Task EnsureRunForReadyOrderAsync(Guid orderId, CancellationToken ct = default);
}
