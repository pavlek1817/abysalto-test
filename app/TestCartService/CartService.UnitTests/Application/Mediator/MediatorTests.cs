using CartService.Application.Mediator.Interfaces;
using CartService.Application.Mediator.Interfaces.Handlers;
using CartService.Application.Mediator.Interfaces.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace CartService.UnitTests.Application.Mediator;

internal class MediatorTests : TestBase
{
    private IServiceCollection services = null!;

    [SetUp]
    public void SetUp()
    {
        this.services = new ServiceCollection();
        this.services.AddScoped<IMediator, CartService.Application.Mediator.Implementations.Mediator>();
    }

    [Test]
    public async Task SendAsync_CommandOrdinaryCase_ShouldReturnResult()
    {
        // Arrange
        this.services.AddScoped<ICommandHandler<TestCommand, TestCommandResult>, TestCommandHandler>();
        var provider = this.services.BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMediator>();
        var command = new TestCommand { Value = 7 };

        // Act
        var result = await mediator.SendAsync<TestCommand, TestCommandResult>(command, CancellationToken.None);

        // Assert
        result.Value.Should().Be(7);
        result.Success.Should().BeTrue();
    }

    [Test]
    public async Task SendAsync_QueryOrdinaryCase_ShouldReturnResult()
    {
        // Arrange
        this.services.AddScoped<IQueryHandler<TestQuery, TestQueryResult>, TestQueryHandler>();
        var provider = this.services.BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMediator>();
        var query = new TestQuery { Value = 3 };

        // Act
        var result = await mediator.SendAsync<TestQuery, TestQueryResult>(query, CancellationToken.None);

        // Assert
        result.Value.Should().Be(3);
        result.Found.Should().BeTrue();
    }

    [Test]
    public async Task SendAsync_CommandHandlerDoesNotExist_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var provider = this.services.BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMediator>();
        var command = new TestCommand { Value = 1 };

        Func<Task> act = async () =>
            await mediator.SendAsync<TestCommand, TestCommandResult>(command, CancellationToken.None);

        // Act & Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Test]
    public async Task SendAsync_RequestIsNotCommandOrQuery_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var provider = this.services.BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMediator>();
        var request = new UnsupportedRequest();

        Func<Task> act = async () =>
            await mediator.SendAsync<UnsupportedRequest, TestCommandResult>(request, CancellationToken.None);

        // Act & Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    private class TestCommand : ICommand
    {
        public int Value { get; init; }
    }

    private class TestCommandResult
    {
        public int Value { get; init; }

        public bool Success { get; init; }
    }

    private class TestCommandHandler : ICommandHandler<TestCommand, TestCommandResult>
    {
        public Task<TestCommandResult> HandleAsync(TestCommand command, CancellationToken ct)
        {
            return Task.FromResult(new TestCommandResult { Value = command.Value, Success = true });
        }
    }

    private class TestQuery : IQuery
    {
        public int Value { get; init; }
    }

    private class TestQueryResult
    {
        public int Value { get; init; }

        public bool Found { get; init; }
    }

    private class TestQueryHandler : IQueryHandler<TestQuery, TestQueryResult>
    {
        public Task<TestQueryResult> HandleAsync(TestQuery query, CancellationToken ct)
        {
            return Task.FromResult(new TestQueryResult { Value = query.Value, Found = true });
        }
    }

    private class UnsupportedRequest : IRequest
    {
    }
}
