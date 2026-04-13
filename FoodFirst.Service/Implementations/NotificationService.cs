using FoodFirst.Dal.Entities;
using FoodFirst.Repository.Interfaces;
using FoodFirst.Service.Interfaces;

namespace FoodFirst.Service.Implementations;

public class NotificationService(IRepository<Notification> notifications) : INotificationService
{
    public Task<IReadOnlyList<Notification>> GetForUserAsync(Guid userId, CancellationToken ct = default) =>
        notifications.FindAsync(n => n.UserId == userId, ct);

    public async Task MarkAsReadAsync(Guid notificationId, CancellationToken ct = default)
    {
        var notif = await notifications.GetByIdAsync(notificationId, ct)
            ?? throw new KeyNotFoundException($"Notification {notificationId} not found.");
        notif.IsRead = true;
        notifications.Update(notif);
        await notifications.SaveChangesAsync(ct);
    }
}
