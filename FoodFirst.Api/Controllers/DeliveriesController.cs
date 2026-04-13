using FoodFirst.Dto.Deliveries;
using FoodFirst.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodFirst.Api.Controllers;

[ApiController]
[Route("api/deliveries")]
[Authorize]
public class DeliveriesController(IDeliveryService deliveries) : ControllerBase
{
    [HttpGet("mine")]
    public async Task<ActionResult<IReadOnlyList<DeliveryDto>>> Mine(CancellationToken ct) =>
        Ok(await deliveries.GetMineAsync(CurrentUser.Id(User), ct));

    [HttpPut("{id:guid}/pickup")]
    public async Task<IActionResult> Pickup(Guid id, CancellationToken ct)
    {
        await deliveries.PickupAsync(id, ct);
        return NoContent();
    }

    [HttpPut("{id:guid}/location")]
    public async Task<IActionResult> Location(Guid id, UpdateLocationRequest request, CancellationToken ct)
    {
        await deliveries.UpdateLocationAsync(id, request, ct);
        return NoContent();
    }

    [HttpPut("{id:guid}/complete")]
    public async Task<IActionResult> Complete(Guid id, CompleteDeliveryRequest request, CancellationToken ct)
    {
        await deliveries.CompleteAsync(id, request, ct);
        return NoContent();
    }

    [HttpPut("{id:guid}/fail")]
    public async Task<IActionResult> Fail(Guid id, FailDeliveryRequest request, CancellationToken ct)
    {
        await deliveries.FailAsync(id, request, ct);
        return NoContent();
    }
}
