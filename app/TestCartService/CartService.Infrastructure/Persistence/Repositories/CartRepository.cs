using CartService.Application.Persistence;
using CartService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CartService.Infrastructure.Persistence.Repositories;

public class CartRepository(CartDbContext dbContext)
    : Repository<Cart>(dbContext), ICartRepository
{
    public override Task<Cart?> GetAsync(int id)
        => dbContext.Set<Cart>()
            .Include(x => x.Items)
            .SingleOrDefaultAsync(x => x.Id == id);
}