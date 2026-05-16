using CartService.Application.Features.AddToCart;
using FluentValidation;

namespace CartService.Api.Validators;

public class AddToCartCommandValidator : AbstractValidator<AddToCartCommand>
{
    public AddToCartCommandValidator()
    {
        RuleFor(x => x.OwnerId)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.ProductId)
            .GreaterThan(0);

        RuleFor(x => x.Quantity)
            .GreaterThan(0);
    }
}
