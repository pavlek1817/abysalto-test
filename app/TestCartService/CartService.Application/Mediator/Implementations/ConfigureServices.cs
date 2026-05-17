using CartService.Application.Mediator.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CartService.Application.Mediator.Implementations;

public static class ConfigureServices
{
    /// <summary>
    /// Method that adds handler services to the service collection.
    /// </summary>
    /// <param name="services">Specified service collection.</param>
    public static void AddMediatorServices(this IServiceCollection services)
    {
        // Add hame handler
        services.AddScoped<IMediator, Mediator>();
    }
}