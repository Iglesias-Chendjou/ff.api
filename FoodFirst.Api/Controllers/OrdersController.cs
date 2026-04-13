using FoodFirst.Dto.Orders;
using FoodFirst.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodFirst.Api.Controllers;

[ApiController]
[Route("api")]
[Authorize]
public class OrdersController(IOrderService orders) : ControllerBase
{
    [HttpPost("cart/validate")]
    public async Task<ActionResult<CartValidationResponse>> Validate(CartValidationRequest request, CancellationToken ct) =>
        Ok(await orders.ValidateCartAsync(request, ct));

    [HttpPost("orders")]
    public async Task<ActionResult<OrderDto>> Create(CreateOrderRequest request, CancellationToken ct)
    {
        var order = await orders.CreateAsync(CurrentUser.Id(User), request, ct);
        return CreatedAtAction(nameof(Get), new { id = order.Id }, order);
    }

    [HttpGet("orders/mine")]
    public async Task<ActionResult<IReadOnlyList<OrderDto>>> Mine(CancellationToken ct) =>
        Ok(await orders.GetMyOrdersAsync(CurrentUser.Id(User), ct));

    [HttpGet("orders/{id:guid}")]
    public async Task<ActionResult<OrderDto>> Get(Guid id, CancellationToken ct)
    {
        var dto = await orders.GetAsync(id, ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPut("orders/{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, UpdateOrderStatusRequest request, CancellationToken ct)
    {
        await orders.UpdateStatusAsync(id, request.Status, ct);
        return NoContent();
    }
}
