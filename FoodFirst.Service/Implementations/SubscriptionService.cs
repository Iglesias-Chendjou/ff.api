using FoodFirst.Dal.Entities;
using FoodFirst.Dal.Enums;
using FoodFirst.Dto.Subscriptions;
using FoodFirst.Repository.Interfaces;
using FoodFirst.Service.Interfaces;

namespace FoodFirst.Service.Implementations;

public class SubscriptionService(IRepository<Subscription> repo) : ISubscriptionService
{
    public async Task<SubscriptionDto> CreateAsync(Guid clientId, CreateSubscriptionRequest request, CancellationToken ct = default)
    {
        var price = request.Plan switch
        {
            SubscriptionPlan.Monthly => 29.99m,
            SubscriptionPlan.Quarterly => 79.99m,
            SubscriptionPlan.SemiAnnual => 149.99m,
            SubscriptionPlan.Annual => 279.99m,
            _ => 29.99m
        };

        var sub = new Subscription
        {
            Id = Guid.NewGuid(),
            ClientId = clientId,
            PlanType = request.Plan,
            Status = SubscriptionStatus.Active,
            MonthlyPrice = price,
            StartDate = DateTime.UtcNow,
            NextBillingDate = DateTime.UtcNow.AddMonths(1),
            DeliveryAddressId = request.DeliveryAddressId,
            PreferredDeliveryDay = request.PreferredDeliveryDay,
            PreferredCategories = request.PreferredCategories
        };
        await repo.AddAsync(sub, ct);
        await repo.SaveChangesAsync(ct);
        return Map(sub);
    }

    public async Task<SubscriptionDto?> GetMineAsync(Guid clientId, CancellationToken ct = default)
    {
        var sub = await repo.FirstOrDefaultAsync(s => s.ClientId == clientId, ct);
        return sub is null ? null : Map(sub);
    }

    public async Task CancelAsync(Guid clientId, CancellationToken ct = default)
    {
        var sub = await repo.FirstOrDefaultAsync(s => s.ClientId == clientId, ct)
            ?? throw new KeyNotFoundException("Subscription not found.");
        sub.Status = SubscriptionStatus.Cancelled;
        sub.CancelledAt = DateTime.UtcNow;
        repo.Update(sub);
        await repo.SaveChangesAsync(ct);
    }

    private static SubscriptionDto Map(Subscription s) =>
        new(s.Id, s.PlanType, s.Status, s.MonthlyPrice, s.StartDate, s.NextBillingDate);
}
