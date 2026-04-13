using FoodFirst.Dal.Entities;
using FoodFirst.Dal.Enums;
using FoodFirst.Dto.SurpriseBox;
using FoodFirst.Repository.Interfaces;
using FoodFirst.Service.Interfaces;

namespace FoodFirst.Service.Implementations;

public class SurpriseBoxService(
    IRepository<SurpriseBoxPlan> plans,
    IRepository<SurpriseBoxSubscription> subscriptions) : ISurpriseBoxService
{
    public async Task<IReadOnlyList<SurpriseBoxPlanDto>> GetPlansAsync(CancellationToken ct = default)
    {
        var list = await plans.FindAsync(p => p.IsActive, ct);
        return list.Select(p => new SurpriseBoxPlanDto(p.Id, p.Name, p.Description, p.MonthlyPrice, p.DeliveriesPerMonth, p.EstimatedBoxValue)).ToList();
    }

    public async Task SubscribeAsync(Guid clientId, SubscribeSurpriseBoxRequest request, CancellationToken ct = default)
    {
        var plan = await plans.GetByIdAsync(request.PlanId, ct)
            ?? throw new KeyNotFoundException($"Plan {request.PlanId} not found.");

        var sub = new SurpriseBoxSubscription
        {
            Id = Guid.NewGuid(),
            ClientId = clientId,
            SurpriseBoxPlanId = plan.Id,
            Status = SubscriptionStatus.Active,
            DeliveryAddressId = request.DeliveryAddressId,
            StartDate = DateTime.UtcNow,
            CurrentPeriodStart = DateTime.UtcNow,
            CurrentPeriodEnd = DateTime.UtcNow.AddMonths(1),
            NextBillingDate = DateTime.UtcNow.AddMonths(1)
        };
        await subscriptions.AddAsync(sub, ct);
        await subscriptions.SaveChangesAsync(ct);
    }
}
