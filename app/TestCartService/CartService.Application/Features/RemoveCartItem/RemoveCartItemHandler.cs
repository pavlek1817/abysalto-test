using AutoMapper;
using CartService.Application.Exceptions;
using CartService.Application.Features.GetCart.Models;
using CartService.Application.Infrastructure.Persistence;
using CartService.Application.Mediator.Interfaces.Handlers;
using Microsoft.Extensions.Logging;

namespace CartService.Application.Features.RemoveCartItem;

public class RemoveCartItemHandler(
    ILogger<RemoveCartItemHandler> logger,
    ICartRepository cartRepository,
    IMapper mapper) : ICommandHandler<RemoveCartItemCommand, CartModel>
{
    public async Task<CartModel> HandleAsync(RemoveCartItemCommand command, CancellationToken ct)
    {
        var cart = await cartRepository.GetByOwnerIdAsync(command.OwnerId, ct);

        if (cart is null)
        {
            logger.LogWarning("Cart for owner id {OwnerId} was not found.", command.OwnerId);
            throw new ItemDoesNotExistException($"Cart was not found.");
        }

        var item = cart.Items.SingleOrDefault(x => x.ProductId == command.ProductId);

        if (item is null)
        {
            logger.LogWarning("Product {ProductId} was not found in cart for owner {OwnerId}.", command.ProductId, command.OwnerId);
            throw new ItemDoesNotExistException($"Product was not found in cart.");
        }

        cart.Items.Remove(item);
        cart.UpdatedAt = DateTime.UtcNow;

        await cartRepository.UpdateAsync(cart, ct);

        return mapper.Map<CartModel>(cart);
    }
}