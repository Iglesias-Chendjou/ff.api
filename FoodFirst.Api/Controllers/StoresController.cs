using FoodFirst.Dto.Stores;
using FoodFirst.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodFirst.Api.Controllers;

[ApiController]
[Route("api/stores")]
[Authorize]
public class StoresController(IStoreService stores) : ControllerBase
{
    [HttpGet("{id:guid}/catalog")]
    public async Task<ActionResult<IReadOnlyList<StoreInventoryItemDto>>> Catalog(Guid id, CancellationToken ct) =>
        Ok(await stores.GetCatalogAsync(id, ct));

    [HttpPost("{id:guid}/inventory/publish")]
    public async Task<IActionResult> Publish(Guid id, PublishInventoryRequest request, CancellationToken ct)
    {
        await stores.PublishInventoryAsync(id, request, ct);
        return NoContent();
    }
}
