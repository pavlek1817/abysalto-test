using CartService.Application.Exceptions;
using CartService.Application.Features.GetCart;
using CartService.Application.Features.GetCart.Models;
using CartService.Domain.Entities;

namespace CartService.UnitTests.Application.Features.GetCartHandlerTests;

internal class OrdinaryTests : GetCartHandlerTestBase
{
    private int _id;

    [SetUp]
    public void SetUp()
    {
        this._id = this.Fixture.Create<int>();

        this.InstantiatedDependencies();
    }

    [Test]
    public async Task CartExistCase_ShouldReturnCorrectResponse()
    {
        // Arrange
        var cart = this.Fixture.Build<Cart>()
            .With(x => x.Id, _id)
            .Without(x => x.Items)
            .Create();

        var cartItems = new List<CartItem>
        {
            this.Fixture.Build<CartItem>()
                .Without(x => x.Cart)
                .Create(),
            this.Fixture.Build<CartItem>()
                .Without(x => x.Cart)
                .Create(),
            this.Fixture.Build<CartItem>()
                .Without(x => x.Cart)
                .Create()
        };
        
        cart.Items.AddRange(cartItems);

        this.MockedCartRepository
            .Setup(x => x.GetByIdAsync(_id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cart);

        // Act
        var result = await this.GetService()
            .HandleAsync(new GetCartQuery(_id), CancellationToken.None);

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
        this.MockedCartRepository
            .Setup(x => x.GetByIdAsync(_id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => null);

        
        var act = async () => await this.GetService().HandleAsync(new GetCartQuery(_id), CancellationToken.None);

        // Act && Assert
        await act.Should().ThrowAsync<ItemDoesNotExistException>();
    }
}
