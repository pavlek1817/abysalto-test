using CartService.Domain.Entities;

namespace CartService.Application.Contracts;

public interface ICartService
{
    Task<int> AddAsync(string ownerId, CancellationToken ct);
    
    Task<Cart> GetByIdAsync(int id, CancellationToken ct);
    
    Task DeleteByIdAsync(int id, CancellationToken ct);
}