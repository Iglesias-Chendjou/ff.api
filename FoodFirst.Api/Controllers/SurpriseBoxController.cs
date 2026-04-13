using FoodFirst.Dto.SurpriseBox;
using FoodFirst.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodFirst.Api.Controllers;

[ApiController]
[Route("api/surprise-box")]
public class SurpriseBoxController(ISurpriseBoxService surprise) : ControllerBase
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
}
