using AutoMapper;
using CartService.Application.Exceptions;
using CartService.Application.Features.GetCart.Models;
using CartService.Application.Features.RemoveCart.Models;
using CartService.Application.Infrastructure.Persistence;
using CartService.Application.Mediator.Interfaces.Handlers;
using Microsoft.Extensions.Logging;

namespace CartService.Application.Features.RemoveCart;

public class RemoveCartHandler(
    ILogger<RemoveCartHandler> logger,
    ICartRepository cartRepository,
    IMapper mapper) : ICommandHandler<RemoveCartCommand, EmptyResponse>
{
    public async Task<EmptyResponse> HandleAsync(RemoveCartCommand command, CancellationToken ct)
    {
        var cart = await cartRepository.GetByOwnerIdAsync(command.OwnerId, ct);

        if (cart is null)
        {
            logger.LogWarning("Cart for owner id {OwnerId} was not found.", command.OwnerId);
            throw new ItemDoesNotExistException($"Cart was not found.");
        }

        var model = mapper.Map<CartModel>(cart);

        await cartRepository.DeleteAsync(cart.Id, ct);

        return new EmptyResponse();
    }
}
