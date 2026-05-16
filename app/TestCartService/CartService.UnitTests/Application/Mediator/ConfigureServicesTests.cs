using CartService.Application.Mediator.Implementations;
using CartService.Application.Mediator.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CartService.UnitTests.Application.Mediator;

internal class ConfigureServicesTests : TestBase
{
    private IServiceCollection services = null!;

    [SetUp]
    public void SetUp()
    {
        this.services = new ServiceCollection();
    }

    [Test]
    public void AddHandlerServices_OrdinaryCase_ShouldRegisterServicesCorrectly()
    {
        // Arrange
        this.services.AddMediatorServices();

        // Act
        var serviceProvider = this.services.BuildServiceProvider();

        // Assert
        var probabilityFunction = serviceProvider.GetService<IMediator>();
        probabilityFunction.Should().NotBeNull();
        var mediator = serviceProvider.GetService<IMediator>();
        mediator.Should().NotBeNull();
    }
}