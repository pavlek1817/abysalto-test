using CartService.Application.Exceptions;
using CartService.Application.Features.GetCart;
using CartService.Application.Features.Shared.Models;
using CartService.Domain.Entities;

namespace CartService.UnitTests.Application.Features.GetCartHandlerTests;

internal class OrdinaryTests : GetCartHandlerTestBase
{
    private string ownerId;

    [SetUp]
    public void SetUp()
    {
        ownerId = Fixture.Create<string>();

        InstantiatedDependencies();
    }

    [Test]
    public async Task CartExistCase_ShouldReturnCorrectResponse()
    {
        // Arrange
        var cart = Fixture.Build<Cart>()
            .With(x => x.OwnerId, ownerId)
            .Without(x => x.Items)
            .Create();

        var cartItems = new List<CartItem>
        {
            Fixture.Build<CartItem>()
                .Without(x => x.Cart)
                .Create(),
            Fixture.Build<CartItem>()
                .Without(x => x.Cart)
                .Create(),
            Fixture.Build<CartItem>()
                .Without(x => x.Cart)
                .Create()
        };
        
        cart.Items.AddRange(cartItems);

        MockedCartRepository
            .Setup(x => x.GetByOwnerIdAsync(ownerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cart);

        // Act
        var result = await GetService()
            .HandleAsync(new GetCartQuery(ownerId), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(cart.Id);
        result.UpdatedAt.Should().Be(cart.UpdatedAt);
        result.Items.Should().HaveCount(cart.Items.Count);

        result.Items.Should().BeEquivalentTo(
            cartItems.Select(x =>
                new CartItemModel
                {
                    ProductId = x.ProductId,
                    Quantity = x.Quantity, 
                    UnitPrice= x.UnitPrice,
                }));
    }

    [Test]
    public async Task CartDoesNotExistCase_ShouldThrowItemDoesNotExistException()
    {
        // Arrange
        MockedCartRepository
            .Setup(x => x.GetByOwnerIdAsync(ownerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => null);

        
        var act = async () => await GetService().HandleAsync(new GetCartQuery(ownerId), CancellationToken.None);

        // Act && Assert
        await act.Should().ThrowAsync<ItemDoesNotExistException>();
    }
}
