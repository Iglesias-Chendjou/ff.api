using FoodFirst.Dto.Field;

namespace FoodFirst.Service.Interfaces;

public interface IPreparationService
{
    Task<IReadOnlyList<PreparationQueueItemDto>> GetQueueAsync(Guid preparerUserId, CancellationToken ct = default);
    Task StartPreparingAsync(Guid orderId, Guid preparerUserId, CancellationToken ct = default);
    Task MarkReadyAsync(Guid orderId, Guid preparerUserId, CancellationToken ct = default);
}
