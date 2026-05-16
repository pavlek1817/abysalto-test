using CartService.Api.Filters;
using CartService.Application.Exceptions;
using CartService.Application.Features.AddToCart;
using CartService.Application.Features.GetCart;
using CartService.Application.Features.GetCart.Models;
using CartService.Application.Mediator.Interfaces;

namespace CartService.Api.Endpoints;

public static class CartEndpoints
{
    public static void RegisterCartEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("cart")
            .AddEndpointFilter<ValidationFilter>();

        group.MapPost("add", async (
            AddToCartCommand command,
            IMediator mediator,
            CancellationToken ct) =>
        {
            try
            {
                var result = await mediator.SendAsync<AddToCartCommand, CartModel>(command, ct);
                return Results.Ok(result);
            }
            catch (ItemDoesNotExistException ex)
            {
                return Results.BadRequest(ex.Message);
            }
            catch (NotEnoughQuantityException ex)
            {
                return Results.BadRequest(ex.Message);
            }
        });

        group.MapGet("get", async (
            string ownerId,
            IMediator mediator,
            CancellationToken ct) =>
        {
            try
            {
                var result = await mediator.SendAsync<GetCartQuery, CartModel>(new GetCartQuery(ownerId), ct);
                return Results.Ok(result);
            }
            catch (ItemDoesNotExistException)
            {
                return Results.NotFound();
            }
        });
    }
}
