using FoodFirst.Dto.Suppliers;

namespace FoodFirst.Service.Interfaces;

public interface ISupplierService
{
    Task<Guid> RegisterAsync(RegisterSupplierRequest request, CancellationToken ct = default);
    Task<Guid> SubmitBulkPurchaseAsync(Guid supplierId, BulkPurchaseRequestDto request, CancellationToken ct = default);
}
