namespace CartService.Domain.Entities;

public class Cart : IEntity
{
    public int Id { get; init; }
    
    public string OwnerId { get; init; } = string.Empty;
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
    
    public List<CartItem> Items { get; init; } = new ();
}