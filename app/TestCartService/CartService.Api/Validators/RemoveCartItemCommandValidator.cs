using CartService.Application.Features.RemoveCartItem;
using FluentValidation;

namespace CartService.Api.Validators;

public class RemoveCartItemCommandValidator : AbstractValidator<RemoveCartItemCommand>
{
    public RemoveCartItemCommandValidator()
    {
        RuleFor(x => x.CartId)
            .GreaterThan(0);

        RuleFor(x => x.ProductId)
            .GreaterThan(0);
    }
}