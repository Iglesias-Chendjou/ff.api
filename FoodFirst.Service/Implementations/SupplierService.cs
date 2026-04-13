using FoodFirst.Dal.Entities;
using FoodFirst.Dal.Enums;
using FoodFirst.Dto.Suppliers;
using FoodFirst.Repository.Interfaces;
using FoodFirst.Service.Interfaces;

namespace FoodFirst.Service.Implementations;

public class SupplierService(
    IRepository<Supplier> suppliers,
    IRepository<BulkPurchaseRequest> bulkRequests) : ISupplierService
{
    public async Task<Guid> RegisterAsync(RegisterSupplierRequest request, CancellationToken ct = default)
    {
        var supplier = new Supplier
        {
            Id = Guid.NewGuid(),
            CompanyName = request.CompanyName,
            ContactName = request.ContactName,
            Email = request.Email,
            Phone = request.Phone,
            SupplierType = request.SupplierType,
            VATNumber = request.VATNumber,
            Address = request.Address,
            PostalCode = request.PostalCode,
            City = request.City,
            Description = request.Description,
            IsApproved = false,
            CreatedAt = DateTime.UtcNow
        };
        await suppliers.AddAsync(supplier, ct);
        await suppliers.SaveChangesAsync(ct);
        return supplier.Id;
    }

    public async Task<Guid> SubmitBulkPurchaseAsync(Guid supplierId, BulkPurchaseRequestDto request, CancellationToken ct = default)
    {
        var entry = new BulkPurchaseRequest
        {
            Id = Guid.NewGuid(),
            SupplierId = supplierId,
            CategoryId = request.CategoryId,
            ProductDescription = request.ProductDescription,
            Quantity = request.Quantity,
            Unit = request.Unit,
            ProposedPricePerUnit = request.ProposedPricePerUnit,
            ExpirationDate = request.ExpirationDate,
            Status = RequestStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
        await bulkRequests.AddAsync(entry, ct);
        await bulkRequests.SaveChangesAsync(ct);
        return entry.Id;
    }
}
