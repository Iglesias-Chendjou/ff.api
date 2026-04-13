using FoodFirst.Dto.SurpriseBox;

namespace FoodFirst.Service.Interfaces;

public interface ISurpriseBoxService
{
    Task<IReadOnlyList<SurpriseBoxPlanDto>> GetPlansAsync(CancellationToken ct = default);
    Task SubscribeAsync(Guid clientId, SubscribeSurpriseBoxRequest request, CancellationToken ct = default);
}
