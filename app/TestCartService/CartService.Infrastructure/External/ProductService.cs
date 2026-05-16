using CartService.Application.Exceptions;
using CartService.Application.Infrastructure.External;

namespace CartService.Infrastructure.External;

public class ProductService : IProductService
{
    private static readonly Random Random = new();

    private record ProductStock(int Quantity, decimal Price);

    private static readonly Dictionary<int, ProductStock> Products = Enumerable
        .Range(1, 10)
        .ToDictionary(id => id, _ => new ProductStock(
            Quantity: 40,
            Price: (decimal)Random.Next(1, 101)));

    public Task<decimal> ResolveProductPriceAndSubtractAsync(ProductModel product, CancellationToken ct)
    {
        if (!Products.TryGetValue(product.ProductId, out var stock))
            throw new ItemDoesNotExistException(
                $"Product {product.ProductId} does not exist.");

        if (product.Quantity > stock.Quantity)
            throw new NotEnoughQuantityException(
                $"Product {product.ProductId} does not have enough quantity available.");

        // Subtract specified quantity from product.
        Products[product.ProductId] = stock with { Quantity = stock.Quantity - product.Quantity };
        
        return Task.FromResult(Products[product.ProductId].Price);
    }

    public Task SubtractProductStockAsync(ProductModel product, CancellationToken ct)
    {
        if (!Products.TryGetValue(product.ProductId, out var stock))
            throw new ItemDoesNotExistException(
                $"Product {product.ProductId} does not exist.");

        if (product.Quantity > stock.Quantity)
            throw new NotEnoughQuantityException(
                $"Product {product.ProductId} does not have enough quantity available.");

        Products[product.ProductId] = stock with { Quantity = stock.Quantity - product.Quantity };
        return Task.CompletedTask;
    }

    public Task ReplenishProductStockAsync(ProductModel product, CancellationToken ct)
    {
        if (!Products.TryGetValue(product.ProductId, out var stock))
            throw new ItemDoesNotExistException(
                $"Product {product.ProductId} does not exist.");

        Products[product.ProductId] = stock with { Quantity = stock.Quantity + product.Quantity };
        return Task.CompletedTask;
    }
}
