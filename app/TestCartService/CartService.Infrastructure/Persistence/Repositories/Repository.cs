using CartService.Application.Persistence;
using CartService.Domain;
using Microsoft.EntityFrameworkCore;

namespace CartService.Infrastructure.Persistence.Repositories;

public class Repository<TEntity>(CartDbContext dbContext) : IRepository<TEntity>
    where TEntity : class, IEntity, new()
{
    public virtual Task<TEntity?> GetAsync(int id)
        => dbContext.Set<TEntity>()
            .SingleOrDefaultAsync(x => x.Id == id);

    public async Task<TEntity> InsertAsync(TEntity entity)
    {
        var entry = await dbContext.Set<TEntity>().AddAsync(entity);
        await dbContext.SaveChangesAsync();
        return entry.Entity;
    }

    public async Task UpdateAsync(TEntity entity)
    {
        if (dbContext.Entry(entity).State == EntityState.Detached)
            dbContext.Set<TEntity>().Attach(entity);

        dbContext.Entry(entity).State = EntityState.Modified;
        await dbContext.SaveChangesAsync();
    }

    public Task DeleteAsync(int id)
        => dbContext.Set<TEntity>()
            .Where(x => x.Id == id)
            .ExecuteDeleteAsync();
}