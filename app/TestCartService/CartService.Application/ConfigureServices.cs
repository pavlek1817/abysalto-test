using CartService.Application.Contracts.AddToCart;
using CartService.Application.Contracts.GetCart;
using CartService.Application.Contracts.GetCart.Models;
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

        return services;
    }
}