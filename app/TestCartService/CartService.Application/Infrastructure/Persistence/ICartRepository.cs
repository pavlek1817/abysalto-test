using CartService.Domain.Entities;

namespace CartService.Application.Infrastructure.Persistence;

public interface ICartRepository : IRepository<Cart>
{
    /// <summary>
    /// Gets the <see cref="Cart"/> with its items for the specified owner.
    /// </summary>
    /// <param name="ownerId">Owner identifier.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Returns <see cref="Cart"/> instance if exists, else returns null.</returns>
    Task<Cart?> GetByOwnerIdAsync(string ownerId, CancellationToken ct);
}