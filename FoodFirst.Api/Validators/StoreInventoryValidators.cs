using FluentValidation;
using FoodFirst.Dal.Enums;
using FoodFirst.Dto.Stores;

namespace FoodFirst.Api.Validators;

public class UpsertStoreInventoryItemDtoValidator : AbstractValidator<UpsertStoreInventoryItemDto>
{
    public UpsertStoreInventoryItemDtoValidator()
    {
        RuleFor(x => x.ProductTemplateId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThanOrEqualTo(0).LessThanOrEqualTo(10_000);
        RuleFor(x => x.ExpirationDate)
            .Must(d => d > DateTime.UtcNow.Date)
            .WithMessage("La date de péremption doit être strictement postérieure à aujourd'hui.");

        // Reason = Unsellable -> UnsellableSubReason requis
        RuleFor(x => x.UnsellableSubReason)
            .NotNull()
            .When(x => x.Reason == ListingReason.Unsellable)
            .WithMessage("UnsellableSubReason est requis lorsque Reason = Unsellable.");

        // Reason = NearExpiry -> ExpirationDate ≤ demain (DLC J+1)
        RuleFor(x => x.ExpirationDate)
            .Must(d => d.Date <= DateTime.UtcNow.Date.AddDays(1))
            .When(x => x.Reason == ListingReason.NearExpiry)
            .WithMessage("Pour Reason = NearExpiry, la date de péremption doit être J ou J+1.");

        // Reason = NearExpiry -> pas de UnsellableSubReason
        RuleFor(x => x.UnsellableSubReason)
            .Null()
            .When(x => x.Reason == ListingReason.NearExpiry)
            .WithMessage("UnsellableSubReason doit être vide lorsque Reason = NearExpiry.");

        // DiscountPercentOverride ∈ [0, 100]
        RuleFor(x => x.DiscountPercentOverride!.Value)
            .InclusiveBetween(0, 100)
            .When(x => x.DiscountPercentOverride.HasValue)
            .WithMessage("DiscountPercentOverride doit être compris entre 0 et 100.");

        RuleFor(x => x.ReasonNotes).MaximumLength(500);
    }
}

public class UpsertStoreInventoryRequestValidator : AbstractValidator<UpsertStoreInventoryRequest>
{
    public UpsertStoreInventoryRequestValidator()
    {
        RuleFor(x => x.Items).NotEmpty();
        RuleForEach(x => x.Items).SetValidator(new UpsertStoreInventoryItemDtoValidator());
    }
}
