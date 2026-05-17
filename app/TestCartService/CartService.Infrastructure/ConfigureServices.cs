using CartService.Application.Infrastructure.External;
using CartService.Application.Infrastructure.Persistence;
using CartService.Infrastructure.External;
using CartService.Infrastructure.Persistence;
using CartService.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace CartService.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        var sp = services.BuildServiceProvider();

        var config = sp.GetRequiredService<IOptions<InfrastructureConfig>>().Value;

        services.AddDbContext<CartDbContext>(options =>
            options.UseNpgsql(config.Database.ConnectionString));

        services.AddSingleton<IConnectionMultiplexer>(
            ConnectionMultiplexer.Connect(
                ConfigurationOptions.Parse(config.Cache.Host, true)));

        services.AddHealthChecks()
            .AddNpgSql(config.Database.ConnectionString, name: "postgres", tags: ["ready"])
            .AddRedis(config.Cache.Host, name: "redis", tags: ["ready"]);
        
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddSingleton<IProductService, ProductService>();

        return services;
    }
}