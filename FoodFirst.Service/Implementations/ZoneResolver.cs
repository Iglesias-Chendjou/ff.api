using FoodFirst.Dal.Context;
using FoodFirst.Dal.Entities;
using FoodFirst.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodFirst.Service.Implementations;

public class ZoneResolver(AppDbContext db) : IZoneResolver
{
    public async Task<Zone?> ResolveByCommuneAsync(string commune, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(commune)) return null;
        var normalized = commune.Trim();
        var zones = await db.Zones.AsNoTracking().Where(z => z.IsActive).ToListAsync(ct);
        return zones.FirstOrDefault(z => z.Communes.Any(c => string.Equals(c, normalized, StringComparison.OrdinalIgnoreCase)));
    }
}
