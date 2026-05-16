using CartService.Application.Contracts.GetCart.Models;
using CartService.Application.Exceptions;
using CartService.Application.Mediator.Interfaces.Handlers;
using CartService.Application.Persistence;
using Microsoft.Extensions.Logging;

namespace CartService.Application.Contracts.GetCart;

/// <summary>
/// Class that implements method for get cart by id.
/// If card does not exist, throws <see cref="ItemDoesNotExistException"/>.
/// </summary>
/// <param name="logger"></param>
/// <param name="cartRepository"></param>
public class GetCartHandler(
    ILogger<GetCartHandler> logger,
    ICartRepository cartRepository) : IQueryHandler<GetCartQuery, CartModel>
{
    public async Task<CartModel> HandleAsync(GetCartQuery query, CancellationToken ct)
    {
        var cart = await cartRepository.GetByIdAsync(query.Id, ct);

        if (cart is null)
        {
            logger.LogWarning("Cart with id {Id} was not found.", query.Id);
            throw new ItemDoesNotExistException($"Cart was not found.");
        }

        return new CartModel(
            Id: cart.Id,
            CreatedAt: cart.CreatedAt,
            UpdatedAt: cart.UpdatedAt ?? default,
            Items: cart.Items
                .Select(item => new CartItemModel(
                    item.ProductId.ToString(),
                    item.Quantity,
                    item.UnitPrice))
                .ToArray());
    }
}