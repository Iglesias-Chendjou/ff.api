using FoodFirst.Dal.Context;
using FoodFirst.Dal.Enums;
using FoodFirst.Dto.Subscriptions;
using FoodFirst.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodFirst.Api.Controllers;

[ApiController]
[Route("api/subscriptions")]
[Authorize]
public class SubscriptionsController(ISubscriptionService subs, AppDbContext db) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<SubscriptionDto>> Create(CreateSubscriptionRequest request, CancellationToken ct) =>
        Ok(await subs.CreateAsync(CurrentUser.Id(User), request, ct));

    [HttpGet("mine")]
    public async Task<ActionResult<SubscriptionDto>> Mine(CancellationToken ct)
    {
        var dto = await subs.GetMineAsync(CurrentUser.Id(User), ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpDelete("mine")]
    public async Task<IActionResult> Cancel(CancellationToken ct)
    {
        await subs.CancelAsync(CurrentUser.Id(User), ct);
        return NoContent();
    }

    [HttpPut("mine/pause")]
    public async Task<IActionResult> Pause(CancellationToken ct)
    {
        var userId = CurrentUser.Id(User);
        var sub = await db.Subscriptions.FirstOrDefaultAsync(s => s.ClientId == userId, ct);
        if (sub is null) return NotFound();

        sub.Status = SubscriptionStatus.Paused;
        await db.SaveChangesAsync(ct);
        return NoContent();
    }

    [HttpPut("mine/resume")]
    public async Task<IActionResult> Resume(CancellationToken ct)
    {
        var userId = CurrentUser.Id(User);
        var sub = await db.Subscriptions.FirstOrDefaultAsync(s => s.ClientId == userId, ct);
        if (sub is null) return NotFound();

        sub.Status = SubscriptionStatus.Active;
        await db.SaveChangesAsync(ct);
        return NoContent();
    }
}
