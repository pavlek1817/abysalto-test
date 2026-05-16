using CartService.Application.Mediator.Interfaces.Models;

namespace CartService.Application.Features.RemoveCartItem;

public record RemoveCartItemCommand(string OwnerId, int ProductId) : ICommand;