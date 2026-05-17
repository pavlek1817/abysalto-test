using CartService.Api.Filters;
using CartService.Application.Exceptions;
using CartService.Application.Features.AddToCart;
using CartService.Application.Features.GetCart;
using CartService.Application.Features.GetCart.Models;
using CartService.Application.Features.RemoveCart;
using CartService.Application.Features.RemoveCart.Models;
using CartService.Application.Features.RemoveCartItem;
using CartService.Application.Features.SubtractFromCart;
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

        group.MapPut("remove-item", async (
            RemoveCartItemCommand command,
            IMediator mediator,
            CancellationToken ct) =>
        {
            try
            {
                var result = await mediator.SendAsync<RemoveCartItemCommand, CartModel>(command, ct);
                return Results.Ok(result);
            }
            catch (ItemDoesNotExistException ex)
            {
                return Results.NotFound(ex.Message);
            }
        });

        group.MapDelete("{id:int}", async (
            int id,
            IMediator mediator,
            CancellationToken ct) =>
        {
            if (id <= 0)
                return Results.BadRequest("Cart id must be greater than 0.");

            try
            {
                await mediator.SendAsync<RemoveCartCommand, EmptyResponse>(new RemoveCartCommand(id), ct);
                return Results.NoContent();
            }
            catch (ItemDoesNotExistException ex)
            {
                return Results.NotFound(ex.Message);
            }
        });

        group.MapPut("subtract", async (
            SubtractFromCartCommand command,
            IMediator mediator,
            CancellationToken ct) =>
        {
            try
            {
                var result = await mediator.SendAsync<SubtractFromCartCommand, CartModel>(command, ct);
                return Results.Ok(result);
            }
            catch (ItemDoesNotExistException ex)
            {
                return Results.NotFound(ex.Message);
            }
            catch (NotEnoughQuantityException ex)
            {
                return Results.BadRequest(ex.Message);
            }
        });

        group.MapGet("{ownerId}", async (
            string ownerId,
            IMediator mediator,
            CancellationToken ct) =>
        {
            if (string.IsNullOrEmpty(ownerId))
                return Results.BadRequest("Owner id must not be empty.");

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
