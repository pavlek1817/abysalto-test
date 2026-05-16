namespace CartService.Application.Contracts.GetCart.Models;

public record CartItemModel(string ProductId, int Quantity, decimal Price);