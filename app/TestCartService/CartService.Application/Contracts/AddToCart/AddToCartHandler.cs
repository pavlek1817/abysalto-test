using AutoMapper;
using CartService.Application.Contracts.GetCart.Models;
using CartService.Application.Mediator.Interfaces.Handlers;
using CartService.Application.Persistence;
using CartService.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace CartService.Application.Contracts.AddToCart;

public class AddToCartHandler(
    ILogger<AddToCartHandler> logger,
    ICartRepository cartRepository,
    IMapper mapper) : ICommandHandler<AddToCartCommand, CartModel>
{
    public async Task<CartModel> HandleAsync(AddToCartCommand command, CancellationToken ct)
    {
        var cart = await cartRepository.GetByOwnerIdAsync(command.OwnerId, ct);

        if (cart is null)
        {
            logger.LogInformation("No cart found for owner {OwnerId}. Creating a new one.", command.OwnerId);

            cart = await cartRepository.InsertAsync(new Cart
            {
                OwnerId = command.OwnerId,
                Items =
                [
                    new CartItem
                    {
                        ProductId = command.ProductId,
                        Quantity = command.Quantity,
                        UnitPrice = command.UnitPrice
                    }
                ]
            }, ct);
        }
        else
        {
            var existingItem = cart.Items
                .SingleOrDefault(x => x.ProductId == command.ProductId);

            if (existingItem is not null)
            {
                existingItem.Quantity += command.Quantity;
                existingItem.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    CartId = cart.Id,
                    ProductId = command.ProductId,
                    Quantity = command.Quantity,
                    UnitPrice = command.UnitPrice
                });
            }

            await cartRepository.UpdateAsync(cart, ct);
        }

        return mapper.Map<CartModel>(cart);
    }
}
