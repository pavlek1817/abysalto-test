namespace CartService.Domain.Entities;

public class CartItem : IEntity
{
    public int Id { get; init; }
    
    public int CartId { get; init; }
    
    public int ProductId { get; init; }
    
    public int Quantity { get; init; }
    
    public decimal UnitPrice { get; init; }
    
    public DateTime CreatedAt { get; init; }
    
    public DateTime? UpdatedAt { get; init; }

    public Cart Cart { get; init; } = new Cart();
}