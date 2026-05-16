using CartService.Application.Mediator.Interfaces.Models;

namespace CartService.Application.Features.GetCart;

public record GetCartQuery(string OwnerId) : IQuery;