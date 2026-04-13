using FoodFirst.Dal.Enums;

namespace FoodFirst.Dto.Orders;

public record CartItemDto(Guid StoreInventoryId, int Quantity);

public record CartValidationRequest(IReadOnlyList<CartItemDto> Items, Guid DeliveryAddressId);

public record CartValidationResponse(bool IsValid, decimal SubTotal, decimal DeliveryFee, decimal TVA, decimal Total, IReadOnlyList<string> Errors);

public record CreateOrderRequest(IReadOnlyList<CartItemDto> Items, Guid DeliveryAddressId, string? Notes);

public record UpdateOrderStatusRequest(OrderStatus Status);

public record OrderItemDto(Guid Id, string ProductName, int Quantity, decimal UnitPrice, decimal LineTotal, PriceRange PriceRange);

public record OrderDto(
    Guid Id,
    string OrderNumber,
    OrderStatus Status,
    decimal SubTotal,
    decimal DeliveryFee,
    decimal TVA,
    decimal TotalAmount,
    DateTime CreatedAt,
    IReadOnlyList<OrderItemDto> Items);
