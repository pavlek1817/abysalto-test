namespace CartService.Infrastructure.Configs;

public record RedisConfig
{
    public string Host { get; init; } = string.Empty;
}