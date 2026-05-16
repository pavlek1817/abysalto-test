namespace CartService.Domain.Entities;

public class CartItem : IEntity
{
    public int Id { get; init; }
    
    public int CartId { get; init; }
    
    public int ProductId { get; init; }
    
    public int Quantity { get; set; }

    public decimal UnitPrice { get; init; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Cart Cart { get; init; } = new Cart();
}