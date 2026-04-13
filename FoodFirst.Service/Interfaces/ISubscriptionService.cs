using FoodFirst.Dto.Subscriptions;

namespace FoodFirst.Service.Interfaces;

public interface ISubscriptionService
{
    Task<SubscriptionDto> CreateAsync(Guid clientId, CreateSubscriptionRequest request, CancellationToken ct = default);
    Task<SubscriptionDto?> GetMineAsync(Guid clientId, CancellationToken ct = default);
    Task CancelAsync(Guid clientId, CancellationToken ct = default);
}
