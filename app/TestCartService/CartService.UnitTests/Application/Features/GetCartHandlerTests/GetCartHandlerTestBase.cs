using CartService.Application.Features.GetCart;
using CartService.Application.Features.GetCart.Models;
using CartService.Application.Mediator.Interfaces.Handlers;
using CartService.Application.Persistence;
using Microsoft.Extensions.Logging;

namespace CartService.UnitTests.Application.Features.GetCartHandlerTests;

internal class GetCartHandlerTestBase : ServiceTestBase<IQueryHandler<GetCartQuery, CartModel>>
{
    private Mock<ILogger<GetCartHandler>> mockedLogger { get; set; }
    
    protected Mock<ICartRepository> MockedCartRepository { get; private set; }

    protected override IQueryHandler<GetCartQuery, CartModel> GetService()
        => new GetCartHandler(
            this.mockedLogger.Object,
            this.MockedCartRepository.Object,
            this.Mapper);

    protected override void InstantiatedDependencies()
    {
        this.mockedLogger = new Mock<ILogger<GetCartHandler>>();
        this.MockedCartRepository = new Mock<ICartRepository>();
    }
}