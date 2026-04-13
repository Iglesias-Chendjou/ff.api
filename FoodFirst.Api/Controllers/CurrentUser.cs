using System.Security.Claims;

namespace FoodFirst.Api.Controllers;

internal static class CurrentUser
{
    public static Guid Id(ClaimsPrincipal user)
    {
        var sub = user.FindFirstValue("sub") ?? user.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(sub, out var id) ? id : throw new UnauthorizedAccessException();
    }
}
