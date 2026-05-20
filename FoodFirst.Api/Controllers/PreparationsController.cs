using FoodFirst.Dto.Field;
using FoodFirst.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodFirst.Api.Controllers;

[ApiController]
[Route("api")]
[Authorize(Roles = "Preparer")]
public class PreparationsController(IPreparationService preparations) : ControllerBase
{
    [HttpGet("preparations/queue")]
    public async Task<ActionResult<IReadOnlyList<PreparationQueueItemDto>>> Queue(CancellationToken ct) =>
        Ok(await preparations.GetQueueAsync(CurrentUser.Id(User), ct));

    [HttpGet("preparations/orders/{id:guid}")]
    public async Task<ActionResult<PreparationOrderDetailDto>> Order(Guid id, CancellationToken ct)
    {
        var dto = await preparations.GetOrderAsync(id, CurrentUser.Id(User), ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPost("preparations/scan/{code}")]
    public async Task<ActionResult<PreparationOrderDetailDto>> Scan(string code, CancellationToken ct)
    {
        var dto = await preparations.ScanOrderAsync(code, CurrentUser.Id(User), ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPut("orders/{id:guid}/start-preparing")]
    public async Task<IActionResult> StartPreparing(Guid id, CancellationToken ct)
    {
        await preparations.StartPreparingAsync(id, CurrentUser.Id(User), ct);
        return NoContent();
    }

    [HttpPut("orders/{id:guid}/ready")]
    public async Task<IActionResult> MarkReady(Guid id, CancellationToken ct)
    {
        await preparations.MarkReadyAsync(id, CurrentUser.Id(User), ct);
        return NoContent();
    }
}
