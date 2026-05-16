namespace CartService.Domain;

public interface IEntity
{
    int Id { get; init; }
    
    DateTime CreatedAt { get; init; }
}