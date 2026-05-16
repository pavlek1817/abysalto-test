using CartService.Domain.Entities;
using FluentAssertions;

namespace CartService.UnitTests.Infrastructure.Repositories.CartRepositoryTests;

internal class GetByOwnerIdAsyncTests : CartRepositoryTestBase
{
    private int _cartId;
    private string _ownerId = string.Empty;
    private List<Cart> _carts = null!;

    [SetUp]
    public void SetUp()
    {
        this._cartId = this.Fixture.Create<int>();
        this._ownerId = this.Fixture.Create<string>();

        this.InstantiatedDependencies();

        this._carts = new List<Cart>
        {
            new()
            {
                Id = _cartId,
                OwnerId = _ownerId,
                CreatedAt = DateTime.UtcNow,
                Items =
                [
                    this.Fixture.Build<CartItem>()
                        .With(x => x.CartId, _cartId)
                        .Without(x => x.Cart)
                        .Create(),
                    this.Fixture.Build<CartItem>()
                        .With(x => x.CartId, _cartId)
                        .Without(x => x.Cart)
                        .Create()
                ]
            },
            this.Fixture.Build<Cart>()
                .Without(x => x.Items)
                .Create()
        };

        this.DatabaseContext.Set<Cart>().AddRange(_carts);
        this.DatabaseContext.SaveChanges();
    }

    [Test]
    public async Task CartExistCase_ShouldReturnCartWithItems()
    {
        // Act
        var result = await this.GetService().GetByOwnerIdAsync(_ownerId, CancellationToken.None);

        // Assert
        var cart = this._carts.Single(x => x.OwnerId == _ownerId);
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(cart);
    }

    [Test]
    public async Task CartDoesNotExist_ShouldReturnNull()
    {
        // Act
        var result = await this.GetService().GetByOwnerIdAsync(this.Fixture.Create<string>(), CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }
}
