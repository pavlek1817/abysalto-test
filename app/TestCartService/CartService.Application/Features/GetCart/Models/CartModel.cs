namespace CartService.Application.Features.GetCart.Models;

public record CartModel(
    int Id,
    string OwnerId,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    CartItemModel[] Items);