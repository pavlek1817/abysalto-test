using CartService.Application.Mediator.Interfaces.Models;

namespace CartService.Application.Features.SubtractFromCart;

public record SubtractFromCartCommand(int CartId, int ProductId, int Quantity) : ICommand;
