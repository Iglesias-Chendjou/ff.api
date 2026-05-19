using FoodFirst.Dto.Field;
using FoodFirst.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodFirst.Api.Controllers;

[ApiController]
[Route("api")]
[Authorize(Roles = "Collector")]
public class RunsController(ICollectionRunService runs) : ControllerBase
{
    [HttpGet("runs/mine")]
    public async Task<ActionResult<IReadOnlyList<CollectionRunDto>>> Mine(CancellationToken ct) =>
        Ok(await runs.GetMyRunsAsync(CurrentUser.Id(User), ct));

    [HttpGet("runs/{id:guid}")]
    public async Task<ActionResult<CollectionRunDto>> Get(Guid id, CancellationToken ct)
    {
        var dto = await runs.GetByIdAsync(id, CurrentUser.Id(User), ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPut("runs/{id:guid}/start")]
    public async Task<IActionResult> Start(Guid id, CancellationToken ct)
    {
        await runs.StartRunAsync(id, CurrentUser.Id(User), ct);
        return NoContent();
    }

    [HttpPut("runs/{id:guid}/complete")]
    public async Task<IActionResult> Complete(Guid id, CancellationToken ct)
    {
        await runs.CompleteRunAsync(id, CurrentUser.Id(User), ct);
        return NoContent();
    }

    [HttpPut("store-pickups/{id:guid}/complete")]
    public async Task<IActionResult> CompletePickup(Guid id, CompletePickupRequest request, CancellationToken ct)
    {
        await runs.CompletePickupAsync(id, CurrentUser.Id(User), request, ct);
        return NoContent();
    }
}
