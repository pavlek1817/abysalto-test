using CartService.Application.Features.GetCart;
using FluentValidation;

namespace CartService.Api.Validators;

public class GetCartQueryValidator : AbstractValidator<GetCartQuery>
{
    public GetCartQueryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0);
    }
}
