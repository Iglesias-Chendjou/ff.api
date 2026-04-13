using FoodFirst.Dal.Enums;

namespace FoodFirst.Dto.Suppliers;

public record RegisterSupplierRequest(
    string CompanyName,
    string ContactName,
    string Email,
    string Phone,
    SupplierType SupplierType,
    string VATNumber,
    string Address,
    string PostalCode,
    string City,
    string? Description);

public record BulkPurchaseRequestDto(
    Guid? CategoryId,
    string ProductDescription,
    int Quantity,
    string Unit,
    decimal? ProposedPricePerUnit,
    DateTime ExpirationDate);
