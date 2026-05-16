namespace CartService.Domain.Entities;

public class Cart : IEntity
{
    public int Id { get; init; }
    
    public string OwnerId { get; init; } = string.Empty;
    
    public DateTime CreatedAt { get; init; }
    
    public DateTime? UpdatedAt { get; init; }
    
    public List<CartItem> Items { get; init; } = new ();
}