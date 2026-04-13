using FoodFirst.Dal.Entities;

namespace FoodFirst.Service.Interfaces;

public interface INotificationService
{
    Task<IReadOnlyList<Notification>> GetForUserAsync(Guid userId, CancellationToken ct = default);
    Task MarkAsReadAsync(Guid notificationId, CancellationToken ct = default);
}
