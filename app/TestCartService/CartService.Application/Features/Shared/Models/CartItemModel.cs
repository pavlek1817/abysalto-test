namespace CartService.Application.Features.Shared.Models;

public record CartItemModel
{
    public int ProductId { get; init; }
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal TotalPrice => Quantity * UnitPrice;
}