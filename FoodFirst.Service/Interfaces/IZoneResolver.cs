using FoodFirst.Dal.Entities;

namespace FoodFirst.Service.Interfaces;

public interface IZoneResolver
{
    Task<Zone?> ResolveByCommuneAsync(string commune, CancellationToken ct = default);
}
