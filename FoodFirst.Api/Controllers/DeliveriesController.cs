using FoodFirst.Dal.Context;
using FoodFirst.Dto.Deliveries;
using FoodFirst.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodFirst.Api.Controllers;

[ApiController]
[Route("api/deliveries")]
[Authorize]
public class DeliveriesController(IDeliveryService deliveries, AppDbContext db) : ControllerBase
{
    [HttpGet("mine")]
    public async Task<ActionResult<IReadOnlyList<DeliveryDto>>> Mine(CancellationToken ct) =>
        Ok(await deliveries.GetMineAsync(CurrentUser.Id(User), ct));

    [HttpGet("mine/route")]
    public async Task<ActionResult<IReadOnlyList<DeliveryDto>>> Route(CancellationToken ct) =>
        Ok(await deliveries.GetRouteAsync(CurrentUser.Id(User), ct));

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

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var userId = CurrentUser.Id(User);
        var delivery = await db.Deliveries
            .Include(d => d.Order)
            .Include(d => d.DeliveryPerson)
            .FirstOrDefaultAsync(d => d.Id == id, ct);

        if (delivery is null) return NotFound();

        if (delivery.Order.ClientId != userId && delivery.DeliveryPerson.UserId != userId)
            return Forbid();

        return Ok(delivery);
    }

    [HttpGet("{id:guid}/track")]
    public async Task<IActionResult> Track(Guid id, CancellationToken ct)
    {
        var userId = CurrentUser.Id(User);
        var delivery = await db.Deliveries
            .Include(d => d.Order)
            .Include(d => d.DeliveryPerson)
            .FirstOrDefaultAsync(d => d.Id == id, ct);

        if (delivery is null) return NotFound();

        if (delivery.Order.ClientId != userId && delivery.DeliveryPerson.UserId != userId)
            return Forbid();

        return Ok(new
        {
            delivery.Id,
            delivery.Status,
            delivery.CurrentLatitude,
            delivery.CurrentLongitude,
            delivery.EstimatedDeliveryTime
        });
    }

    [HttpPost("{id:guid}/rate")]
    public async Task<IActionResult> Rate(Guid id, [FromBody] RateDeliveryRequest request, CancellationToken ct)
    {
        var userId = CurrentUser.Id(User);
        var delivery = await db.Deliveries
            .Include(d => d.Order)
            .FirstOrDefaultAsync(d => d.Id == id, ct);

        if (delivery is null) return NotFound();

        if (delivery.Order.ClientId != userId)
            return Forbid();

        delivery.ClientRating = request.Rating;
        delivery.ClientComment = request.Comment;
        await db.SaveChangesAsync(ct);
        return Ok(delivery);
    }
}

public record RateDeliveryRequest(int Rating, string? Comment);
