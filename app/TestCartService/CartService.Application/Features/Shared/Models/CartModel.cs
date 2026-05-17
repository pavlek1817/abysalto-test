namespace CartService.Application.Features.Shared.Models;

public record CartModel(
    int Id,
    string OwnerId,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    CartItemModel[] Items)
{
    public decimal TotalPrice => Items.Sum(x => x.TotalPrice);
}