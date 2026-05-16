using CartService.Application.Mediator.Interfaces.Models;

namespace CartService.Application.Contracts.AddToCart;

public record AddToCartCommand(
    string OwnerId,
    int ProductId,
    int Quantity,
    decimal UnitPrice) : ICommand;
