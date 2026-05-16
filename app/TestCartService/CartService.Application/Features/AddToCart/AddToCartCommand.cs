using CartService.Application.Mediator.Interfaces.Models;

namespace CartService.Application.Features.AddToCart;

public record AddToCartCommand(
    string OwnerId,
    int ProductId,
    int Quantity,
    decimal UnitPrice) : ICommand;
