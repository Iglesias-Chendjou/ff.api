using FoodFirst.Dto.Payments;
using FoodFirst.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodFirst.Api.Controllers;

[ApiController]
[Route("api/payments")]
[Authorize]
public class PaymentsController(IPaymentService payments) : ControllerBase
{
    [HttpPost("intent")]
    public async Task<ActionResult<PaymentIntentDto>> CreateIntent(CreatePaymentIntentRequest request, CancellationToken ct) =>
        Ok(await payments.CreateIntentAsync(CurrentUser.Id(User), request.OrderId, ct));

    [HttpPost("mock-confirm/{paymentIntentId}")]
    public async Task<ActionResult<PaymentDto>> ConfirmMock(string paymentIntentId, CancellationToken ct) =>
        Ok(await payments.ConfirmMockAsync(paymentIntentId, ct));

    [HttpGet("by-order/{orderId:guid}")]
    public async Task<ActionResult<PaymentDto>> GetByOrder(Guid orderId, CancellationToken ct)
    {
        var dto = await payments.GetByOrderAsync(orderId, ct);
        return dto is null ? NotFound() : Ok(dto);
    }
}
