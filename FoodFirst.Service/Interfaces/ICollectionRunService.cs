using FoodFirst.Dto.Field;

namespace FoodFirst.Service.Interfaces;

public interface ICollectionRunService
{
    Task<IReadOnlyList<CollectionRunDto>> GetMyRunsAsync(Guid collectorUserId, CancellationToken ct = default);
    Task<CollectionRunDto?> GetByIdAsync(Guid runId, Guid collectorUserId, CancellationToken ct = default);
    Task StartRunAsync(Guid runId, Guid collectorUserId, CancellationToken ct = default);
    Task CompleteRunAsync(Guid runId, Guid collectorUserId, CancellationToken ct = default);
    Task CompletePickupAsync(Guid pickupId, Guid collectorUserId, CompletePickupRequest request, CancellationToken ct = default);
}
