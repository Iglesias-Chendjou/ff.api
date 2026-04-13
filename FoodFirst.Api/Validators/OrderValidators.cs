using FluentValidation;
using FoodFirst.Dto.Orders;

namespace FoodFirst.Api.Validators;

public class CartItemDtoValidator : AbstractValidator<CartItemDto>
{
    public CartItemDtoValidator()
    {
        RuleFor(x => x.StoreInventoryId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0).LessThanOrEqualTo(100);
    }
}

public class CartValidationRequestValidator : AbstractValidator<CartValidationRequest>
{
    public CartValidationRequestValidator()
    {
        RuleFor(x => x.Items).NotEmpty();
        RuleForEach(x => x.Items).SetValidator(new CartItemDtoValidator());
        RuleFor(x => x.DeliveryAddressId).NotEmpty();
    }
}

public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderRequestValidator()
    {
        RuleFor(x => x.Items).NotEmpty();
        RuleForEach(x => x.Items).SetValidator(new CartItemDtoValidator());
        RuleFor(x => x.DeliveryAddressId).NotEmpty();
        RuleFor(x => x.Notes).MaximumLength(500);
    }
}
