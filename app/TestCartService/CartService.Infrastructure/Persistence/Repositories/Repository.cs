using CartService.Application.Persistence;
using CartService.Domain;
using Microsoft.EntityFrameworkCore;

namespace CartService.Infrastructure.Persistence.Repositories;

public class Repository<TEntity>(CartDbContext dbContext) : IRepository<TEntity>
    where TEntity : class, IEntity, new()
{
    public virtual Task<TEntity?> GetByIdAsync(int id, CancellationToken ct)
        => dbContext.Set<TEntity>()
            .SingleOrDefaultAsync(x => x.Id == id, ct);

    public async Task<TEntity> InsertAsync(TEntity entity, CancellationToken ct)
    {
        var entry = await dbContext.Set<TEntity>().AddAsync(entity, ct);
        await dbContext.SaveChangesAsync(ct);
        return entry.Entity;
    }

    public async Task UpdateAsync(TEntity entity, CancellationToken ct)
    {
        if (dbContext.Entry(entity).State == EntityState.Detached)
            dbContext.Set<TEntity>().Attach(entity);

        dbContext.Entry(entity).State = EntityState.Modified;
        await dbContext.SaveChangesAsync(ct);
    }

    public Task DeleteAsync(int id, CancellationToken ct)
        => dbContext.Set<TEntity>()
            .Where(x => x.Id == id)
            .ExecuteDeleteAsync(ct);
}
