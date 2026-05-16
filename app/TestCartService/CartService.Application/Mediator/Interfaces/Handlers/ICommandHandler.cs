using CartService.Application.Mediator.Interfaces.Models;

namespace CartService.Application.Mediator.Interfaces.Handlers;

/// <summary>
/// Represent a generic command handler for handling
/// a command of type <typeparamref name="TCommand"/>
/// and return a response of type <typeparamref name="TResponse"/>.
/// It usually changes the state of object in the data source.
/// </summary>
/// <typeparam name="TCommand">Command model.</typeparam>
/// <typeparam name="TResponse">Response model.</typeparam>
public interface ICommandHandler<in TCommand, TResponse>
    where TCommand : IRequest
{
    /// <summary>
    /// Method that handles command and returns response.
    /// </summary>
    /// <param name="command">Instance of <typeparamref name="TCommand"/>.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Returns an instance of <typeparamref name="TResponse"/>.</returns>
    Task<TResponse> HandleAsync(TCommand command, CancellationToken ct);
}