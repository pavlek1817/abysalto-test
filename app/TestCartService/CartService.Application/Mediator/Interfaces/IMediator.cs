using CartService.Application.Mediator.Interfaces.Models;

namespace CartService.Application.Mediator.Interfaces;

/// <summary>
/// Interface for custom CQRS requests.
/// </summary>
public interface IMediator
{
    /// <summary>
    /// Method that handle the specified command asynchronously.
    /// </summary>
    /// <param name="command">Specified request for the method.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <typeparam name="TRequest">Specified request model.</typeparam>
    /// <typeparam name="TResponse">Specified response model.</typeparam>
    /// <returns>Returns the response of type <typeparamref name="TResponse"/>.</returns>
    /// <exception cref="InvalidOperationException">If implementation for specific type do not exist.</exception>
    Task<TResponse> SendAsync<TRequest, TResponse>(TRequest command, CancellationToken ct)
        where TRequest : IRequest;
}