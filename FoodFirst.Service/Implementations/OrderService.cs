using FoodFirst.Dal.Context;
using FoodFirst.Dal.Entities;
using FoodFirst.Dal.Enums;
using FoodFirst.Dto.Orders;
using FoodFirst.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodFirst.Service.Implementations;

public class OrderService(AppDbContext db) : IOrderService
{
    private const decimal DeliveryFee = 3.50m;
    private const decimal TvaRate = 0.06m;

    public async Task<CartValidationResponse> ValidateCartAsync(CartValidationRequest request, CancellationToken ct = default)
    {
        var (priced, errors) = await PriceCartAsync(request.Items, ct);
        var subTotal = priced.Sum(p => p.LineTotal);
        var tva = Math.Round(subTotal * TvaRate, 2);
        var total = subTotal + DeliveryFee + tva;
        return new CartValidationResponse(errors.Count == 0, subTotal, DeliveryFee, tva, total, errors);
    }

    public async Task<OrderDto> CreateAsync(Guid clientId, CreateOrderRequest request, CancellationToken ct = default)
    {
        await using var tx = await db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable, ct);

        var (priced, errors) = await PriceCartAsync(request.Items, ct);
        if (errors.Count > 0)
            throw new InvalidOperationException(string.Join("; ", errors));

        foreach (var line in priced)
        {
            var affected = await db.StoreInventories
                .Where(si => si.Id == line.InventoryId && si.AvailableQuantity >= line.Quantity && si.IsPublished)
                .ExecuteUpdateAsync(s => s.SetProperty(x => x.AvailableQuantity, x => x.AvailableQuantity - line.Quantity), ct);

            if (affected == 0)
                throw new InvalidOperationException($"Stock unavailable for inventory {line.InventoryId}.");
        }

        var subTotal = priced.Sum(p => p.LineTotal);
        var tva = Math.Round(subTotal * TvaRate, 2);

        var order = new Order
        {
            Id = Guid.NewGuid(),
            OrderNumber = $"FF-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}",
            ClientId = clientId,
            DeliveryAddressId = request.DeliveryAddressId,
            Status = OrderStatus.Pending,
            SubTotal = subTotal,
            DeliveryFee = DeliveryFee,
            TVA = tva,
            TotalAmount = subTotal + DeliveryFee + tva,
            Notes = request.Notes,
            CreatedAt = DateTime.UtcNow,
            Items = priced.Select(p => new OrderItem
            {
                Id = Guid.NewGuid(),
                StoreInventoryId = p.InventoryId,
                StoreId = p.StoreId,
                ProductName = p.ProductName,
                PriceRange = p.PriceRange,
                UnitPrice = p.UnitPrice,
                Quantity = p.Quantity,
                LineTotal = p.LineTotal
            }).ToList()
        };

        db.Orders.Add(order);
        await db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        return Map(order);
    }

    public async Task<OrderDto?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var order = await db.Orders.AsNoTracking()
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id, ct);
        return order is null ? null : Map(order);
    }

    public async Task<IReadOnlyList<OrderDto>> GetMyOrdersAsync(Guid clientId, CancellationToken ct = default)
    {
        var list = await db.Orders.AsNoTracking()
            .Where(o => o.ClientId == clientId)
            .Include(o => o.Items)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(ct);
        return list.Select(Map).ToList();
    }

    public async Task UpdateStatusAsync(Guid id, OrderStatus status, CancellationToken ct = default)
    {
        var order = await db.Orders.FirstOrDefaultAsync(o => o.Id == id, ct)
            ?? throw new KeyNotFoundException($"Order {id} not found.");
        order.Status = status;
        switch (status)
        {
            case OrderStatus.Paid: order.PaidAt = DateTime.UtcNow; break;
            case OrderStatus.Preparing:
            case OrderStatus.ReadyForCollection: order.PreparedAt = DateTime.UtcNow; break;
            case OrderStatus.Collected: order.CollectedAt = DateTime.UtcNow; break;
            case OrderStatus.Delivered: order.DeliveredAt = DateTime.UtcNow; break;
            case OrderStatus.Cancelled: order.CancelledAt = DateTime.UtcNow; break;
        }
        await db.SaveChangesAsync(ct);
    }

    private async Task<(List<PricedLine> Priced, List<string> Errors)> PriceCartAsync(IReadOnlyList<CartItemDto> items, CancellationToken ct)
    {
        var errors = new List<string>();
        var priced = new List<PricedLine>();
        if (items.Count == 0) return (priced, errors);

        var ids = items.Select(i => i.StoreInventoryId).Distinct().ToArray();
        var inventories = await db.StoreInventories.AsNoTracking()
            .Include(si => si.ProductTemplate)
            .Where(si => ids.Contains(si.Id))
            .ToDictionaryAsync(si => si.Id, ct);

        foreach (var item in items)
        {
            if (!inventories.TryGetValue(item.StoreInventoryId, out var inv))
            {
                errors.Add($"Inventory {item.StoreInventoryId} not found.");
                continue;
            }
            if (!inv.IsPublished || inv.AvailableQuantity < item.Quantity)
            {
                errors.Add($"Insufficient stock for inventory {item.StoreInventoryId}.");
                continue;
            }
            var baseline = inv.SelectedRange switch
            {
                PriceRange.Low => inv.ProductTemplate.PriceLowRange,
                PriceRange.Mid => inv.ProductTemplate.PriceMidRange,
                PriceRange.High => inv.ProductTemplate.PriceHighRange,
                _ => inv.ProductTemplate.PriceMidRange
            };
            var unit = Math.Round(baseline * (100 - inv.ProductTemplate.DiscountPercent) / 100m, 2);
            priced.Add(new PricedLine(
                inv.Id, inv.StoreId, inv.ProductTemplate.Name, inv.SelectedRange,
                unit, item.Quantity, unit * item.Quantity));
        }
        return (priced, errors);
    }

    private static OrderDto Map(Order o) => new(
        o.Id, o.OrderNumber, o.Status, o.SubTotal, o.DeliveryFee, o.TVA, o.TotalAmount, o.CreatedAt,
        o.Items.Select(i => new OrderItemDto(i.Id, i.ProductName, i.Quantity, i.UnitPrice, i.LineTotal, i.PriceRange)).ToList());

    private sealed record PricedLine(Guid InventoryId, Guid StoreId, string ProductName, PriceRange PriceRange, decimal UnitPrice, int Quantity, decimal LineTotal);
}
