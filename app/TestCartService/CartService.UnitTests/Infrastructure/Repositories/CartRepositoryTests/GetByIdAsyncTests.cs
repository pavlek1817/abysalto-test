using CartService.Domain.Entities;
using FluentAssertions;

namespace CartService.UnitTests.Infrastructure.Repositories.CartRepositoryTests;

internal class GetByIdAsyncTests : CartRepositoryTestBase
{
    private int _id;
    
    private List<Cart> _carts;

    [SetUp]
    public void SetUp()
    {
        this._id = this.Fixture.Create<int>();

        this.InstantiatedDependencies();

        this._carts = new List<Cart>
        {
            new()
            {
                Id = _id,
                OwnerId = this.Fixture.Create<string>(),
                CreatedAt = DateTime.UtcNow,
                Items =
                [
                    this.Fixture.Build<CartItem>()
                        .With(x => x.CartId, _id)
                        .Without(x => x.Cart)
                        .Create(),
                    this.Fixture.Build<CartItem>()
                        .With(x => x.CartId, _id)
                        .Without(x => x.Cart)
                        .Create()
                ]
            },
            this.Fixture.Build<Cart>()
                .With(x => x.Id, _id + 1)
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
        var result = await this.GetService().GetByIdAsync(_id, CancellationToken.None);

        // Assert

        var cart = this._carts.Single(x => x.Id == _id);
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(cart);
    }

    [Test]
    public async Task CartDoesNotExist_ShouldReturnNull()
    {
        // Act
        var result = await this.GetService().GetByIdAsync(_id + 2, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }
}
