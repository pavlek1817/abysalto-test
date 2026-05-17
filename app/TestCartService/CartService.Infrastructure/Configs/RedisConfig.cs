namespace CartService.Infrastructure.Configs;

public record RedisConfig
{
    public string Host { get; init; } = string.Empty;
    
    public int Port { get; init; }

    public int CacheExpirationInMinutes { get; init; } = 2;
}