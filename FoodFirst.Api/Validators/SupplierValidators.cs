using FluentValidation;
using FoodFirst.Dto.Suppliers;

namespace FoodFirst.Api.Validators;

public class RegisterSupplierRequestValidator : AbstractValidator<RegisterSupplierRequest>
{
    public RegisterSupplierRequestValidator()
    {
        RuleFor(x => x.CompanyName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.ContactName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Phone).NotEmpty().MaximumLength(20);
        RuleFor(x => x.VATNumber).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Address).NotEmpty().MaximumLength(256);
        RuleFor(x => x.PostalCode).NotEmpty().MaximumLength(10);
        RuleFor(x => x.City).NotEmpty().MaximumLength(100);
    }
}

public class BulkPurchaseRequestDtoValidator : AbstractValidator<BulkPurchaseRequestDto>
{
    public BulkPurchaseRequestDtoValidator()
    {
        RuleFor(x => x.ProductDescription).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.Unit).NotEmpty().MaximumLength(30);
        RuleFor(x => x.ExpirationDate).GreaterThan(DateTime.UtcNow);
        RuleFor(x => x.ProposedPricePerUnit).GreaterThanOrEqualTo(0).When(x => x.ProposedPricePerUnit.HasValue);
    }
}
