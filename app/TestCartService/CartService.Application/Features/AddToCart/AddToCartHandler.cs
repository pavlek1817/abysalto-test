using AutoMapper;
using CartService.Application.Features.Shared.Models;
using CartService.Application.Infrastructure.External;
using CartService.Application.Infrastructure.Persistence;
using CartService.Application.Mediator.Interfaces.Handlers;
using CartService.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace CartService.Application.Features.AddToCart;

public class AddToCartHandler(
    ILogger<AddToCartHandler> logger,
    ICartRepository cartRepository,
    IProductService productService,
    IMapper mapper) : ICommandHandler<AddToCartCommand, CartModel>
{
    public async Task<CartModel> HandleAsync(AddToCartCommand command, CancellationToken ct)
    {
        var product = new ProductModel(command.ProductId, command.Quantity);

        var unitPrice = await productService.ResolveProductPriceAndSubtractAsync(product, ct);

        try
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
                            UnitPrice = unitPrice
                        }
                    ]
                }, ct);
            }
            else
            {
                cart.UpdatedAt = DateTime.UtcNow;
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
                        UnitPrice = unitPrice
                    });
                }

                await cartRepository.UpdateAsync(cart, ct);
            }

            return mapper.Map<CartModel>(cart);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, 
                "Failed to add product {ProductId} to cart for owner {OwnerId}. " +
                "Replenishing stock.",
                command.ProductId, command.OwnerId);

            await productService.ReplenishProductStockAsync(product, ct);
            throw;
        }
    }
}
