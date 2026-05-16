namespace CartService.Domain.Entities;

public class Cart : IEntity
{
    public int Id { get; init; }
    
    public string OwnerId { get; init; } = string.Empty;
}