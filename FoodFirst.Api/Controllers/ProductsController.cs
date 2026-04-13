using FoodFirst.Dto.Products;
using FoodFirst.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace FoodFirst.Api.Controllers;

[ApiController]
[Route("api/products")]
[Authorize]
public class ProductsController(IProductService products, IOpenFoodFactsClient openFoodFacts) : ControllerBase
{
    [HttpGet("available")]
    public async Task<ActionResult<IReadOnlyList<AvailableProductDto>>> GetAvailable([FromQuery] Guid zone, CancellationToken ct) =>
        Ok(await products.GetAvailableByZoneAsync(zone, ct));

    [HttpGet("lookup")]
    [AllowAnonymous]
    [EnableRateLimiting("off")]
    public async Task<ActionResult<OpenFoodFactsProductDto>> Lookup([FromQuery] string barcode, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(barcode)) return BadRequest("barcode is required");
        var product = await openFoodFacts.GetByBarcodeAsync(barcode, ct);
        return product is null ? NotFound() : Ok(product);
    }

    [HttpGet("search")]
    [AllowAnonymous]
    [EnableRateLimiting("off")]
    public async Task<ActionResult<OpenFoodFactsSearchResult>> Search(
        [FromQuery] string? brand,
        [FromQuery] string? category,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken ct = default) =>
        Ok(await openFoodFacts.SearchAsync(brand, category, page, pageSize, ct));
}
