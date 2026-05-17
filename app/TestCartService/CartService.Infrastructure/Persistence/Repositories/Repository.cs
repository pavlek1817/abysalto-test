using CartService.Application.Infrastructure.Persistence;
using CartService.Domain;
using Microsoft.EntityFrameworkCore;

namespace CartService.Infrastructure.Persistence.Repositories;

public class Repository<TEntity>(CartDbContext dbContext) : IRepository<TEntity>
    where TEntity : class, IEntity, new()
{
    protected readonly CartDbContext DbContext = dbContext;
    
    public virtual Task<TEntity?> GetByIdAsync(int id, CancellationToken ct)
        => DbContext.Set<TEntity>()
            .SingleOrDefaultAsync(x => x.Id == id, ct);

    public virtual async Task<TEntity> InsertAsync(TEntity entity, CancellationToken ct)
    {
        entity.CreatedAt = DateTime.UtcNow;
        var entry = await DbContext.Set<TEntity>().AddAsync(entity, ct);
        await DbContext.SaveChangesAsync(ct);
        return entry.Entity;
    }

    public virtual async Task UpdateAsync(TEntity entity, CancellationToken ct)
    {
        if (DbContext.Entry(entity).State == EntityState.Detached)
            DbContext.Set<TEntity>().Attach(entity);

        DbContext.Entry(entity).State = EntityState.Modified;
        await DbContext.SaveChangesAsync(ct);
    }

    public virtual async Task DeleteAsync(int id, CancellationToken ct)
    {
        var entity = await DbContext.Set<TEntity>().FindAsync([id], ct);
        if (entity is not null)
            DbContext.Set<TEntity>().Remove(entity);
        await DbContext.SaveChangesAsync(ct);
    }
}
