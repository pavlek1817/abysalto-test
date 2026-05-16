using CartService.Application.Infrastructure.Persistence;
using CartService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CartService.Infrastructure.Persistence.Repositories;

public class CartRepository(CartDbContext dbContext)
    : Repository<Cart>(dbContext), ICartRepository
{
    public override Task<Cart?> GetByIdAsync(int id, CancellationToken ct)
        => dbContext.Set<Cart>()
            .Include(x => x.Items)
            .SingleOrDefaultAsync(x => x.Id == id, ct);

    public Task<Cart?> GetByOwnerIdAsync(string ownerId, CancellationToken ct)
        => dbContext.Set<Cart>()
            .Include(x => x.Items)
            .SingleOrDefaultAsync(x => x.OwnerId == ownerId, ct);
}