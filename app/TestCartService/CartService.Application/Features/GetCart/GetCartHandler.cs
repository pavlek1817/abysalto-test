using AutoMapper;
using CartService.Application.Exceptions;
using CartService.Application.Features.GetCart.Models;
using CartService.Application.Infrastructure.Persistence;
using CartService.Application.Mediator.Interfaces.Handlers;
using Microsoft.Extensions.Logging;

namespace CartService.Application.Features.GetCart;

/// <summary>
/// Class that implements method for get cart by id.
/// If card does not exist, throws <see cref="ItemDoesNotExistException"/>.
/// </summary>
/// <param name="logger"></param>
/// <param name="cartRepository"></param>
/// <param name="mapper"></param>
public class GetCartHandler(
    ILogger<GetCartHandler> logger,
    ICartRepository cartRepository,
    IMapper mapper) : IQueryHandler<GetCartQuery, CartModel>
{
    public async Task<CartModel> HandleAsync(GetCartQuery query, CancellationToken ct)
    {
        var cart = await cartRepository.GetByOwnerIdAsync(query.OwnerId, ct);

        if (cart is null)
        {
            logger.LogWarning("Cart for owner id {OwnerId} was not found.", query.OwnerId);
            throw new ItemDoesNotExistException($"Cart was not found.");
        }

        return mapper.Map<CartModel>(cart);
    }
}