using CartService.Application.Features.GetCart;
using CartService.Application.Features.Shared.Models;
using CartService.Application.Infrastructure.Persistence;
using CartService.Application.Mediator.Interfaces.Handlers;
using Microsoft.Extensions.Logging;

namespace CartService.UnitTests.Application.Features.GetCartHandlerTests;

internal class GetCartHandlerTestBase : ServiceTestBase<IQueryHandler<GetCartQuery, CartModel>>
{
    private Mock<ILogger<GetCartHandler>> mockedLogger { get; set; }
    
    protected Mock<ICartRepository> MockedCartRepository { get; private set; }

    protected override IQueryHandler<GetCartQuery, CartModel> GetService()
        => new GetCartHandler(
            mockedLogger.Object,
            MockedCartRepository.Object,
            Mapper);

    protected override void InstantiatedDependencies()
    {
        mockedLogger = new Mock<ILogger<GetCartHandler>>();
        MockedCartRepository = new Mock<ICartRepository>();
    }
}