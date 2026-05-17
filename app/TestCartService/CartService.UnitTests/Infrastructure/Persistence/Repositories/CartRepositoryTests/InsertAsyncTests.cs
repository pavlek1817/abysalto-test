using CartService.Domain.Entities;

namespace CartService.UnitTests.Infrastructure.Persistence.Repositories.CartRepositoryTests;

internal class InsertAsyncTests : CartRepositoryTestBase
{
    private Cart _cart = null!;
    private Cart _existingCart = null!;

    [SetUp]
    public void SetUp()
    {
        InstantiatedDependencies();

        _existingCart = Fixture.Build<Cart>()
            .Without(x => x.Items)
            .Create();

        DatabaseContext.Set<Cart>().Add(_existingCart);
        DatabaseContext.SaveChanges();

        _cart = Fixture.Build<Cart>()
            .Without(x => x.Items)
            .Create();

        CaptureCartAddedToCache($"cart:{_cart.OwnerId}");
    }

    [Test]
    public async Task OrdinaryCase_ShouldInsertCart_InDbAndCache()
    {
        // Act
        var result = await GetService().InsertAsync(_cart, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(_cart, options => options.Excluding(x => x.CreatedAt));

        var inserted = await DatabaseContext.Set<Cart>().FindAsync(result.Id);
        inserted.Should().NotBeNull();

        var untouched = await DatabaseContext.Set<Cart>().FindAsync(_existingCart.Id);
        untouched.Should().BeEquivalentTo(_existingCart);
        
        AssertResponse(inserted, CapturedCartAddedInCache);
    }
}
