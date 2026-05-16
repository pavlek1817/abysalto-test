using CartService.Application.Features.SubtractFromCart;
using FluentValidation;

namespace CartService.Api.Validators;

public class SubtractFromCartCommandValidator : AbstractValidator<SubtractFromCartCommand>
{
    public SubtractFromCartCommandValidator()
    {
        RuleFor(x => x.CartId)
            .GreaterThan(0);

        RuleFor(x => x.ProductId)
            .GreaterThan(0);

        RuleFor(x => x.Quantity)
            .GreaterThan(0);
    }
}