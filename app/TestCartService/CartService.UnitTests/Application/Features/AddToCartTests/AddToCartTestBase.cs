using CartService.Application.Features.AddToCart;
using CartService.Application.Features.Shared.Models;
using CartService.Application.Infrastructure.External;
using CartService.Application.Infrastructure.Persistence;
using CartService.Application.Mediator.Interfaces.Handlers;
using Microsoft.Extensions.Logging;

namespace CartService.UnitTests.Application.Features.AddToCartTests;

internal class AddToCartTestBase : ServiceTestBase<ICommandHandler<AddToCartCommand, CartModel>>
{
    private Mock<ILogger<AddToCartHandler>> mockedLogger { get; set; } = null!;

    protected Mock<IProductService> MockedProductService { get; set; } = null!;

    protected Mock<ICartRepository> MockedCartRepository { get; set; } = null!;

    protected override ICommandHandler<AddToCartCommand, CartModel> GetService()
        => new AddToCartHandler(
            mockedLogger.Object,
            MockedCartRepository.Object,
            MockedProductService.Object,
            Mapper);

    protected override void InstantiatedDependencies()
    {
        mockedLogger = new Mock<ILogger<AddToCartHandler>>();
        MockedProductService = new Mock<IProductService>();
        MockedCartRepository = new Mock<ICartRepository>();
    }
}