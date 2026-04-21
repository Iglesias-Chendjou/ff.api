using FoodFirst.Dal.Context;
using FoodFirst.Dal.Enums;
using FoodFirst.Dto.SurpriseBox;
using FoodFirst.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodFirst.Api.Controllers;

[ApiController]
[Route("api/surprise-box")]
public class SurpriseBoxController(ISurpriseBoxService surprise, AppDbContext db) : ControllerBase
{
    [HttpGet("plans")]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<SurpriseBoxPlanDto>>> Plans(CancellationToken ct) =>
        Ok(await surprise.GetPlansAsync(ct));

    [HttpPost("subscribe")]
    [Authorize]
    public async Task<IActionResult> Subscribe(SubscribeSurpriseBoxRequest request, CancellationToken ct)
    {
        await surprise.SubscribeAsync(CurrentUser.Id(User), request, ct);
        return NoContent();
    }

    [HttpGet("mine")]
    [Authorize]
    public async Task<IActionResult> GetMine(CancellationToken ct)
    {
        var userId = CurrentUser.Id(User);
        var sub = await db.SurpriseBoxSubscriptions
            .Include(s => s.Plan)
            .FirstOrDefaultAsync(s => s.ClientId == userId, ct);
        return sub is null ? NotFound() : Ok(sub);
    }

    [HttpDelete("mine")]
    [Authorize]
    public async Task<IActionResult> CancelMine(CancellationToken ct)
    {
        var userId = CurrentUser.Id(User);
        var sub = await db.SurpriseBoxSubscriptions
            .FirstOrDefaultAsync(s => s.ClientId == userId, ct);
        if (sub is null) return NotFound();

        sub.Status = SubscriptionStatus.Cancelled;
        sub.CancelledAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/rate")]
    [Authorize]
    public async Task<IActionResult> Rate(Guid id, [FromBody] RateSurpriseBoxRequest request, CancellationToken ct)
    {
        var userId = CurrentUser.Id(User);
        var order = await db.Orders.FirstOrDefaultAsync(o => o.Id == id, ct);
        if (order is null) return NotFound();
        if (order.ClientId != userId) return Forbid();

        order.Notes = $"Rating: {request.Rating}, Comment: {request.Comment}";
        await db.SaveChangesAsync(ct);
        return Ok(order);
    }
}

public record RateSurpriseBoxRequest(int Rating, string? Comment);
