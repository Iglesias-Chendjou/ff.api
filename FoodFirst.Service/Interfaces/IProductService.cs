using FoodFirst.Dto.Products;

namespace FoodFirst.Service.Interfaces;

public interface IProductService
{
    Task<IReadOnlyList<AvailableProductDto>> GetAvailableByZoneAsync(Guid zoneId, CancellationToken ct = default);
    Task<IReadOnlyList<AvailableProductDto>> GetAllAvailableAsync(CancellationToken ct = default);
}
