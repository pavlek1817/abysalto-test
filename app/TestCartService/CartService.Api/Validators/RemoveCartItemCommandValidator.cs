using CartService.Application.Features.RemoveCartItem;
using FluentValidation;

namespace CartService.Api.Validators;

public class RemoveCartItemCommandValidator : AbstractValidator<RemoveCartItemCommand>
{
    public RemoveCartItemCommandValidator()
    {
        RuleFor(x => x.OwnerId)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.ProductId)
            .GreaterThan(0);
    }
}