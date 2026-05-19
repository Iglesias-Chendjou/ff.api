using FoodFirst.Dto.Memberships;

namespace FoodFirst.Service.Interfaces;

public interface IMembershipService
{
    Task<MembershipDto?> GetMineAsync(Guid userId, CancellationToken ct = default);
    Task<MembershipDto> SubscribeAsync(Guid userId, CancellationToken ct = default);
    Task CancelAsync(Guid userId, CancellationToken ct = default);
}
