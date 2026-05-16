namespace CartService.Application.Features.GetCart.Models;

public record CartModel(
    int Id,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    CartItemModel[] Items);