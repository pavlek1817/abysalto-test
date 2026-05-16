using CartService.Application.Mediator.Interfaces.Models;

namespace CartService.Application.Contracts.GetCart;

public record GetCartQuery(int Id) : IQuery;