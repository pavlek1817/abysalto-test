using System.Text.Json;
using System.Text.Json.Serialization;
using CartService.Application.Infrastructure.Persistence;
using CartService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace CartService.Infrastructure.Persistence.Repositories;

public class CartRepository(
    IOptions<InfrastructureConfig> config,
    CartDbContext dbContext,
    IConnectionMultiplexer redis)
    : Repository<Cart>(dbContext), ICartRepository
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        ReferenceHandler = ReferenceHandler.IgnoreCycles
    };

    private IDatabase Cache => redis.GetDatabase();
    private static string CacheKey(string ownerId) => $"cart:{ownerId}";

    public override async Task<Cart?> GetByIdAsync(int id, CancellationToken ct)
    {
        var cart = await DbContext.Set<Cart>()
            .Include(x => x.Items)
            .SingleOrDefaultAsync(x => x.Id == id, ct);

        return cart;
    }

    public async Task<Cart?> GetByOwnerIdAsync(string ownerId, CancellationToken ct)
    {
        var cached = await Cache.StringGetAsync(CacheKey(ownerId));
        if (cached.HasValue)
            return JsonSerializer.Deserialize<Cart>(cached!, JsonOptions);

        var cart = await DbContext.Set<Cart>()
            .Include(x => x.Items)
            .SingleOrDefaultAsync(x => x.OwnerId == ownerId, ct);

        if (cart is not null)
            await SetCacheAsync(cart);

        return cart;
    }

    public override async Task<Cart> InsertAsync(Cart entity, CancellationToken ct)
    {
        var cart = await base.InsertAsync(entity, ct);
        await SetCacheAsync(cart);
        return cart;
    }

    public override async Task UpdateAsync(Cart entity, CancellationToken ct)
    {
        await base.UpdateAsync(entity, ct);
        await SetCacheAsync(entity);
    }

    public override async Task DeleteAsync(int id, CancellationToken ct)
    {
        var cart = await DbContext.Set<Cart>().FindAsync([id], ct);
        if (cart is not null)
        {
            DbContext.Set<Cart>().Remove(cart);
            await DbContext.SaveChangesAsync(ct);
            await Cache.KeyDeleteAsync(CacheKey(cart.OwnerId));
        }
    }

    private Task SetCacheAsync(Cart cart)
        => Cache.StringSetAsync(CacheKey(cart.OwnerId),
            JsonSerializer.Serialize(cart, JsonOptions),
            TimeSpan.FromMinutes(config.Value.Cache.CacheExpirationInMinutes));
}
