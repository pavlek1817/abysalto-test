using CartService.Infrastructure.Persistence;
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
            ConnectionMultiplexer.Connect(config.Cache.Host));

        return services;
    }
}