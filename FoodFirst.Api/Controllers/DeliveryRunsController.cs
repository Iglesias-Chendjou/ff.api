using FoodFirst.Dto.Field;
using FoodFirst.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodFirst.Api.Controllers;

[ApiController]
[Route("api/delivery-runs")]
[Authorize(Roles = "Delivery")]
public class DeliveryRunsController(IDeliveryRunService runs) : ControllerBase
{
    [HttpGet("mine")]
    public async Task<ActionResult<IReadOnlyList<DeliveryRunDto>>> Mine(CancellationToken ct) =>
        Ok(await runs.GetMyRunsAsync(CurrentUser.Id(User), ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<DeliveryRunDto>> Get(Guid id, CancellationToken ct)
    {
        var dto = await runs.GetByIdAsync(id, CurrentUser.Id(User), ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpGet("by-code/{code}")]
    public async Task<ActionResult<DeliveryRunDto>> ByCode(string code, CancellationToken ct)
    {
        var dto = await runs.GetByCodeAsync(code, CurrentUser.Id(User), ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPut("{id:guid}/start")]
    public async Task<IActionResult> Start(Guid id, CancellationToken ct)
    {
        await runs.StartRunAsync(id, CurrentUser.Id(User), ct);
        return NoContent();
    }

    [HttpPut("{id:guid}/complete")]
    public async Task<IActionResult> Complete(Guid id, CancellationToken ct)
    {
        await runs.CompleteRunAsync(id, CurrentUser.Id(User), ct);
        return NoContent();
    }
}
