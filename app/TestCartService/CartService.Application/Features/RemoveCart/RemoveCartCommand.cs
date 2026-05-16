using CartService.Application.Mediator.Interfaces.Models;

namespace CartService.Application.Features.RemoveCart;

public record RemoveCartCommand(string OwnerId) : ICommand;