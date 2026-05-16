using CartService.Infrastructure;

namespace CartService.Api;

public record AppConfig
{
    public InfrastructureConfig Infrastructure { get; init; } = null!;
}