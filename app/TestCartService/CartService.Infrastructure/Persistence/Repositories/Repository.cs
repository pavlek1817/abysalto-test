using CartService.Application.Infrastructure.Persistence;
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
        entity.CreatedAt = DateTime.UtcNow;
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

    public async Task DeleteAsync(int id, CancellationToken ct)
    {
        var entity = await dbContext.Set<TEntity>().FindAsync([id], ct);
        if (entity is not null)
            dbContext.Set<TEntity>().Remove(entity);
        await dbContext.SaveChangesAsync(ct);
    }
}
