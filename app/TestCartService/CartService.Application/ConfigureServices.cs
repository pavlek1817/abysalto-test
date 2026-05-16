using CartService.Application.Features.AddToCart;
using CartService.Application.Features.GetCart;
using CartService.Application.Features.GetCart.Models;
using CartService.Application.Features.RemoveCartItem;
using CartService.Application.Mappings;
using CartService.Application.Mediator.Implementations;
using CartService.Application.Mediator.Interfaces.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace CartService.Application;

public static class ConfigureServices
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(CartMappingProfile).Assembly);

        services.AddMediatorServices();

        services.AddScoped<IQueryHandler<GetCartQuery, CartModel>, GetCartHandler>();
        services.AddScoped<ICommandHandler<AddToCartCommand, CartModel>, AddToCartHandler>();
        services.AddScoped<ICommandHandler<RemoveCartItemCommand, CartModel>, RemoveCartItemHandler>();

        return services;
    }
}