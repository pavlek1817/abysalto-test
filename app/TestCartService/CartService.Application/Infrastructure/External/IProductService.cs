namespace CartService.Application.Infrastructure.External;

/// <summary>
/// This service contains a simulation of external product service.
/// </summary>
public interface IProductService
{
    /// <summary>
    /// Resolves the unit price for the specified product, validates that sufficient
    /// quantity is available, and subtracts the requested quantity from stock.
    /// It is a simulation of an HTTP client call.
    /// </summary>
    /// <param name="product">Product identifier and requested quantity.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Unit price of the product.</returns>
    /// <exception cref="Exceptions.ItemDoesNotExistException">Thrown when the product does not exist.</exception>
    /// <exception cref="Exceptions.NotEnoughQuantityException">Thrown when the product does not have sufficient quantity available.</exception>
    Task<decimal> ResolveProductPriceAndSubtractAsync(ProductModel product, CancellationToken ct);

    /// <summary>
    /// Subtracts the specified quantity from the product's stock.
    /// </summary>
    /// <param name="product">Product identifier and quantity to subtract.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <exception cref="Exceptions.ItemDoesNotExistException">Thrown when the product does not exist.</exception>
    /// <exception cref="Exceptions.NotEnoughQuantityException">Thrown when the product does not have sufficient quantity available.</exception>
    Task SubtractProductStockAsync(ProductModel product, CancellationToken ct);

    /// <summary>
    /// Replenishes the product's stock by restoring the specified quantity.
    /// </summary>
    /// <param name="product">Product identifier and quantity to restore.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <exception cref="Exceptions.ItemDoesNotExistException">Thrown when the product does not exist.</exception>
    Task ReplenishProductStockAsync(ProductModel product, CancellationToken ct);
}
