using CartService.Application.Mediator.Interfaces;
using CartService.Application.Mediator.Interfaces.Handlers;
using CartService.Application.Mediator.Interfaces.Models;
using Microsoft.Extensions.DependencyInjection;

namespace CartService.Application.Mediator.Implementations;

public class Mediator(IServiceProvider serviceProvider)
    : IMediator
{
    public async Task<TResponse> SendAsync<TRequest, TResponse>(TRequest request, CancellationToken ct)
        where TRequest : IRequest
    {
        if (request is ICommand)
        {
            var handler = serviceProvider.GetRequiredService<ICommandHandler<TRequest, TResponse>>();
            return await handler.HandleAsync(request, ct);
        }

        if (request is IQuery)
        {
            var handler = serviceProvider.GetRequiredService<IQueryHandler<TRequest, TResponse>>();

            return await handler.HandleAsync(request, ct);
        }

        throw new InvalidOperationException($"Request type {typeof(TRequest)} does not implement ICommand or IQuery.");
    }
}