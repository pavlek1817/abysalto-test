using CartService.Application.Exceptions;
using CartService.Application.Features.RemoveCart.Models;
using CartService.Application.Infrastructure.External;
using CartService.Application.Infrastructure.Persistence;
using CartService.Application.Mediator.Interfaces.Handlers;
using Microsoft.Extensions.Logging;

namespace CartService.Application.Features.RemoveCart;

public class RemoveCartHandler(
    ILogger<RemoveCartHandler> logger,
    ICartRepository cartRepository,
    IProductService productService) : ICommandHandler<RemoveCartCommand, EmptyResponse>
{
    public async Task<EmptyResponse> HandleAsync(RemoveCartCommand command, CancellationToken ct)
    {
        var cart = await cartRepository.GetByIdAsync(command.Id, ct);

        if (cart is null)
        {
            logger.LogWarning("Cart with id {Id} was not found.", command.Id);
            throw new ItemDoesNotExistException($"Cart was not found.");
        }

        await cartRepository.DeleteAsync(cart.Id, ct);

        await Task.WhenAll(cart.Items.Select(item =>
            productService.ReplenishProductStockAsync(new ProductModel(item.ProductId, item.Quantity), ct)));

        return new EmptyResponse();
    }
}