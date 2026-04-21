using FoodFirst.Dal.Context;
using FoodFirst.Dto.Products;
using FoodFirst.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

namespace FoodFirst.Api.Controllers;

[ApiController]
[Route("api/products")]
[Authorize]
public class ProductsController(IProductService products, IOpenFoodFactsClient openFoodFacts, AppDbContext db) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet("available")]
    public async Task<ActionResult<IReadOnlyList<AvailableProductDto>>> GetAvailable([FromQuery] Guid? zone, CancellationToken ct)
    {
        if (zone is null || zone == Guid.Empty)
        {
            // Return all available products across all zones
            var items = await db.StoreInventories
                .Include(si => si.ProductTemplate).ThenInclude(pt => pt.Category)
                .Include(si => si.Store)
                .Where(si => si.IsPublished && si.AvailableQuantity > 0 && si.ExpirationDate > DateTime.UtcNow && si.Store.IsActive)
                .Take(100)
                .ToListAsync(ct);

            var dtos = items.Select(si =>
            {
                var pt = si.ProductTemplate;
                var price = si.SelectedRange switch
                {
                    Dal.Enums.PriceRange.Low => pt.PriceLowRange,
                    Dal.Enums.PriceRange.High => pt.PriceHighRange,
                    _ => pt.PriceMidRange
                };
                return new AvailableProductDto(
                    si.Id, pt.Id, pt.Name, pt.Description, pt.ImageUrl, pt.Unit,
                    pt.Category?.Name ?? "", si.SelectedRange,
                    price, Math.Round(price * (1 - pt.DiscountPercent / 100m), 2),
                    si.AvailableQuantity, si.ExpirationDate, si.Store.Id, si.Store.Name);
            }).ToList();

            return Ok(dtos);
        }

        return Ok(await products.GetAvailableByZoneAsync(zone.Value, ct));
    }

    [AllowAnonymous]
    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories(CancellationToken ct)
    {
        var categories = await db.ProductCategories
            .Where(c => c.IsActive)
            .OrderBy(c => c.SortOrder)
            .ToListAsync(ct);
        return Ok(categories);
    }

    [AllowAnonymous]
    [HttpGet("zones")]
    public async Task<IActionResult> GetZones(CancellationToken ct)
    {
        var zones = await db.Zones
            .Where(z => z.IsActive)
            .ToListAsync(ct);
        return Ok(zones);
    }

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
