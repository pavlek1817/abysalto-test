using CartService.Domain.Entities;

namespace CartService.UnitTests.Infrastructure.Persistence.Repositories.CartRepositoryTests;

internal class GetByIdAsyncTests : CartRepositoryTestBase
{
    private int _id;

    [SetUp]
    public void SetUp()
    {
        _id = Fixture.Create<int>();

        InstantiatedDependencies();

        Carts = new List<Cart>
        {
            new()
            {
                Id = _id,
                OwnerId = Fixture.Create<string>(),
                CreatedAt = DateTime.UtcNow,
                Items =
                [
                    Fixture.Build<CartItem>()
                        .With(x => x.CartId, _id)
                        .Without(x => x.Cart)
                        .Create(),
                    Fixture.Build<CartItem>()
                        .With(x => x.CartId, _id)
                        .Without(x => x.Cart)
                        .Create()
                ]
            },
            Fixture.Build<Cart>()
                .With(x => x.Id, _id + 1)
                .Without(x => x.Items)
                .Create()
        };

        DatabaseContext.Set<Cart>().AddRange(Carts);
        DatabaseContext.SaveChanges();
    }

    [Test]
    public async Task CartExistCase_ShouldReturnCartWithItems()
    {
        // Act
        var result = await GetService().GetByIdAsync(_id, CancellationToken.None);

        // Assert

        var cart = Carts.Single(x => x.Id == _id);
        AssertResponse(cart, result);
    }

    [Test]
    public async Task CartDoesNotExist_ShouldReturnNull()
    {
        // Act
        var result = await GetService().GetByIdAsync(_id + 2, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }
}
