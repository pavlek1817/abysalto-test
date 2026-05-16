using CartService.Application.Mediator.Interfaces.Models;

namespace CartService.Application.Mediator.Interfaces.Handlers;

/// <summary>
/// Represent a generic query handler for retrieving items of type <typeparamref name="TResponse"/>
/// from data source for specified <typeparamref name="TQuery"/>.
/// It does not change the state of any object.
/// </summary>
/// <typeparam name="TQuery">Query model.</typeparam>
/// <typeparam name="TResponse">Response model.</typeparam>
public interface IQueryHandler<in TQuery, TResponse>
    where TQuery : IRequest
{
    /// <summary>
    /// Method that handles the query and returns the response.
    /// </summary>
    /// <param name="query">Query instance.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Returns instance of <typeparamref name="TResponse"/>.</returns>
    Task<TResponse> HandleAsync(TQuery query, CancellationToken ct);
}