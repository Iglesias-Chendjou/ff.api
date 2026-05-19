using FoodFirst.Dto.Memberships;
using FoodFirst.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodFirst.Api.Controllers;

[ApiController]
[Route("api/memberships")]
[Authorize]
public class MembershipsController(IMembershipService memberships) : ControllerBase
{
    [HttpGet("mine")]
    public async Task<ActionResult<MembershipDto>> Mine(CancellationToken ct)
    {
        var dto = await memberships.GetMineAsync(CurrentUser.Id(User), ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<MembershipDto>> Subscribe(CancellationToken ct) =>
        Ok(await memberships.SubscribeAsync(CurrentUser.Id(User), ct));

    [HttpDelete("mine")]
    public async Task<IActionResult> Cancel(CancellationToken ct)
    {
        await memberships.CancelAsync(CurrentUser.Id(User), ct);
        return NoContent();
    }
}
