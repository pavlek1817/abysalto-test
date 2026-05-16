using CartService.Domain;

namespace CartService.Application.Persistence;

public interface IRepository<TEntity> where TEntity : class, IEntity, new()
{
    /// <summary>
    /// Method that gets the <see cref="TEntity"/> by id.
    /// </summary>
    /// <param name="id">Id.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Returns <see cref="TEntity"/> instence if exists, else returns null.</returns>
    Task<TEntity?> GetByIdAsync(int id, CancellationToken ct);

    /// <summary>
    /// Inserts a new <see cref="TEntity"/> into the data store.
    /// </summary>
    /// <param name="entity">The entity to insert.</param>
    /// <param name="ct">Cancellation token.</param>
    Task<TEntity> InsertAsync(TEntity entity, CancellationToken ct);

    /// <summary>
    /// Updates an existing <see cref="TEntity"/> in the data store.
    /// </summary>
    /// <param name="entity">The entity with updated values.</param>
    /// <param name="ct">Cancellation token.</param>
    Task UpdateAsync(TEntity entity, CancellationToken ct);

    /// <summary>
    /// Deletes the <see cref="TEntity"/> with the specified id from the data store.
    /// </summary>
    /// <param name="id">Id of the entity to delete.</param>
    /// <param name="ct">Cancellation token.</param>
    Task DeleteAsync(int id, CancellationToken ct);
}