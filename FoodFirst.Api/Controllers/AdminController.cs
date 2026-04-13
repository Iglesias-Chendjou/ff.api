using FoodFirst.Dal.Entities;
using FoodFirst.Dal.Enums;
using FoodFirst.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodFirst.Api.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminController(
    IRepository<Supplier> suppliers,
    IRepository<BulkPurchaseRequest> bulkRequests) : ControllerBase
{
    [HttpPost("suppliers/{id:guid}/approve")]
    public async Task<IActionResult> ApproveSupplier(Guid id, CancellationToken ct)
    {
        var supplier = await suppliers.GetByIdAsync(id, ct);
        if (supplier is null) return NotFound();
        supplier.IsApproved = true;
        supplier.ApprovedAt = DateTime.UtcNow;
        suppliers.Update(supplier);
        await suppliers.SaveChangesAsync(ct);
        return NoContent();
    }

    [HttpGet("bulk-requests")]
    public async Task<IActionResult> ListBulkRequests(CancellationToken ct) =>
        Ok(await bulkRequests.FindAsync(r => r.Status == RequestStatus.Pending || r.Status == RequestStatus.UnderReview, ct));
}
