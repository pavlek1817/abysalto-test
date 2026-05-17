using AutoMapper;
using CartService.Application.Exceptions;
using CartService.Application.Features.GetCart.Models;
using CartService.Application.Infrastructure.External;
using CartService.Application.Infrastructure.Persistence;
using CartService.Application.Mediator.Interfaces.Handlers;
using Microsoft.Extensions.Logging;

namespace CartService.Application.Features.SubtractFromCart;

public class SubtractFromCartHandler(
    ILogger<SubtractFromCartHandler> logger,
    ICartRepository cartRepository,
    IProductService productService,
    IMapper mapper) : ICommandHandler<SubtractFromCartCommand, CartModel>
{
    public async Task<CartModel> HandleAsync(SubtractFromCartCommand command, CancellationToken ct)
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

        if (command.Quantity > item.Quantity)
        {
            logger.LogWarning("Cannot subtract {Quantity} from product {ProductId} in cart {CartId}, only {Available} available.",
                command.Quantity, command.ProductId, command.CartId, item.Quantity);
            throw new NotEnoughQuantityException($"Cannot subtract {command.Quantity} units, only {item.Quantity} available.");
        }

        item.Quantity -= command.Quantity;
        item.UpdatedAt = DateTime.UtcNow;

        if (item.Quantity == 0)
            cart.Items.Remove(item);

        cart.UpdatedAt = DateTime.UtcNow;

        await cartRepository.UpdateAsync(cart, ct);

        await productService.ReplenishProductStockAsync(new ProductModel(command.ProductId, command.Quantity), ct);

        return mapper.Map<CartModel>(cart);
    }
}