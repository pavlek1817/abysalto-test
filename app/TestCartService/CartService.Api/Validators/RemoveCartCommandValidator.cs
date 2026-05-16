using CartService.Application.Features.RemoveCart;
using FluentValidation;

namespace CartService.Api.Validators;

public class RemoveCartCommandValidator : AbstractValidator<RemoveCartCommand>
{
    public RemoveCartCommandValidator()
    {
        RuleFor(x => x.OwnerId)
            .NotNull()
            .NotEmpty();
    }
}