using FoodFirst.Dal.Context;
using FoodFirst.Dal.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodFirst.Api.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController(AppDbContext db) : ControllerBase
{
    [HttpGet("me")]
    public async Task<IActionResult> GetMe(CancellationToken ct)
    {
        var userId = CurrentUser.Id(User);
        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId, ct);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateMe([FromBody] UpdateMeRequest request, CancellationToken ct)
    {
        var userId = CurrentUser.Id(User);
        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId, ct);
        if (user is null) return NotFound();

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.Phone = request.Phone;
        user.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);
        return Ok(user);
    }

    [HttpGet("me/addresses")]
    public async Task<IActionResult> GetAddresses(CancellationToken ct)
    {
        var userId = CurrentUser.Id(User);
        var addresses = await db.Addresses
            .Where(a => a.UserId == userId)
            .ToListAsync(ct);
        return Ok(addresses);
    }

    [HttpPost("me/addresses")]
    public async Task<IActionResult> CreateAddress([FromBody] CreateAddressRequest request, CancellationToken ct)
    {
        var userId = CurrentUser.Id(User);
        var address = new Address
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Street = request.Street,
            Number = request.Number,
            PostalCode = request.PostalCode,
            City = request.City,
            Commune = request.Commune,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            IsDefault = request.IsDefault,
            Label = request.Label
        };

        db.Addresses.Add(address);
        await db.SaveChangesAsync(ct);
        return Created($"api/users/me/addresses/{address.Id}", address);
    }

    [HttpPut("me/addresses/{id:guid}")]
    public async Task<IActionResult> UpdateAddress(Guid id, [FromBody] UpdateAddressRequest request, CancellationToken ct)
    {
        var userId = CurrentUser.Id(User);
        var address = await db.Addresses.FirstOrDefaultAsync(a => a.Id == id, ct);
        if (address is null) return NotFound();
        if (address.UserId != userId) return Forbid();

        address.Street = request.Street;
        address.Number = request.Number;
        address.PostalCode = request.PostalCode;
        address.City = request.City;
        address.Commune = request.Commune;
        address.Latitude = request.Latitude;
        address.Longitude = request.Longitude;
        address.IsDefault = request.IsDefault;
        address.Label = request.Label;

        await db.SaveChangesAsync(ct);
        return Ok(address);
    }

    [HttpDelete("me/addresses/{id:guid}")]
    public async Task<IActionResult> DeleteAddress(Guid id, CancellationToken ct)
    {
        var userId = CurrentUser.Id(User);
        var address = await db.Addresses.FirstOrDefaultAsync(a => a.Id == id, ct);
        if (address is null) return NotFound();
        if (address.UserId != userId) return Forbid();

        db.Addresses.Remove(address);
        await db.SaveChangesAsync(ct);
        return NoContent();
    }

    [HttpPut("me/addresses/{id:guid}/default")]
    public async Task<IActionResult> SetDefaultAddress(Guid id, CancellationToken ct)
    {
        var userId = CurrentUser.Id(User);
        var address = await db.Addresses.FirstOrDefaultAsync(a => a.Id == id, ct);
        if (address is null) return NotFound();
        if (address.UserId != userId) return Forbid();

        var allAddresses = await db.Addresses.Where(a => a.UserId == userId).ToListAsync(ct);
        foreach (var a in allAddresses)
            a.IsDefault = a.Id == id;

        await db.SaveChangesAsync(ct);
        return NoContent();
    }
}

public record UpdateMeRequest(string FirstName, string LastName, string Phone);

public record CreateAddressRequest(
    string Street,
    string Number,
    string PostalCode,
    string City,
    string Commune,
    decimal Latitude,
    decimal Longitude,
    bool IsDefault,
    string? Label);

public record UpdateAddressRequest(
    string Street,
    string Number,
    string PostalCode,
    string City,
    string Commune,
    decimal Latitude,
    decimal Longitude,
    bool IsDefault,
    string? Label);
