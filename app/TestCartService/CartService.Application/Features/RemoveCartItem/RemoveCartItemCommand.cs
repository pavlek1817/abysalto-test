using CartService.Application.Mediator.Interfaces.Models;

namespace CartService.Application.Features.RemoveCartItem;

public record RemoveCartItemCommand(int CartId, int ProductId) : ICommand;