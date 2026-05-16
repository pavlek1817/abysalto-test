using CartService.Infrastructure.Configs;

namespace CartService.Infrastructure;

public record InfrastructureConfig
{
    public RedisConfig Cache { get; init; } = null!;
    public PostgreeConfig Database { get; init; } = null!;
}
