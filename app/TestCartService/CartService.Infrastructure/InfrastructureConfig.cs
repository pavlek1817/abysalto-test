using CartService.Infrastructure.Configs;

namespace CartService.Infrastructure;

public record InfrastructureConfig(RedisConfig Cache, PostgreeConfig Database);
