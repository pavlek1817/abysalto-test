namespace CartService.Infrastructure.Configs;

public record PostgreeConfig
{
    public string ConnectionString { get; init; } = string.Empty;
}