using FoodFirst.Dto.Suppliers;
using FoodFirst.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodFirst.Api.Controllers;

[ApiController]
[Route("api/suppliers")]
public class SuppliersController(ISupplierService suppliers) : ControllerBase
{
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<Guid>> Register(RegisterSupplierRequest request, CancellationToken ct) =>
        Ok(await suppliers.RegisterAsync(request, ct));

    [HttpPost("{supplierId:guid}/bulk-offers")]
    [Authorize(Roles = "Admin,Supplier")]
    public async Task<ActionResult<Guid>> SubmitBulkOffer(Guid supplierId, BulkPurchaseRequestDto request, CancellationToken ct) =>
        Ok(await suppliers.SubmitBulkPurchaseAsync(supplierId, request, ct));
}
