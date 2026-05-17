using AutoMapper;
using CartService.Application.Exceptions;
using CartService.Application.Features.Shared.Models;
using CartService.Application.Infrastructure.External;
using CartService.Application.Infrastructure.Persistence;
using CartService.Application.Mediator.Interfaces.Handlers;
using Microsoft.Extensions.Logging;

namespace CartService.Application.Features.RemoveCartItem;

public class RemoveCartItemHandler(
    ILogger<RemoveCartItemHandler> logger,
    ICartRepository cartRepository,
    IProductService productService,
    IMapper mapper) : ICommandHandler<RemoveCartItemCommand, CartModel>
{
    public async Task<CartModel> HandleAsync(RemoveCartItemCommand command, CancellationToken ct)
    {
        var cart = await cartRepository.GetByIdAsync(command.CartId, ct);

        if (cart is null)
        {
            logger.LogWarning("Cart with id {CartId} was not found.", command.CartId);
            throw new ItemDoesNotExistException($"Cart was not found.");
        }

        var item = cart.Items.SingleOrDefault(x => x.ProductId == command.ProductId);

        if (item is null)
        {
            logger.LogWarning("Product {ProductId} was not found in cart {CartId}.", command.ProductId, command.CartId);
            throw new ItemDoesNotExistException($"Product was not found in cart.");
        }

        cart.Items.Remove(item);
        cart.UpdatedAt = DateTime.UtcNow;

        await cartRepository.UpdateAsync(cart, ct);

        await productService.ReplenishProductStockAsync(new ProductModel(item.ProductId, item.Quantity), ct);

        return mapper.Map<CartModel>(cart);
    }
}