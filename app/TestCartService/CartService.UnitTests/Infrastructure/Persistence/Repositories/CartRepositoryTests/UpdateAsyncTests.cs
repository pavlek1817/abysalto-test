using CartService.Domain.Entities;

namespace CartService.UnitTests.Infrastructure.Persistence.Repositories.CartRepositoryTests;

internal class UpdateAsyncTests : CartRepositoryTestBase
{
    private Cart _cart = null!;
    private Cart _irrelevantCart = null!;

    [SetUp]
    public void SetUp()
    {
        InstantiatedDependencies();
        
        CaptureCartAddedToCache();

        _cart = Fixture.Build<Cart>()
            .Without(x => x.Items)
            .Create();

        _irrelevantCart = Fixture.Build<Cart>()
            .Without(x => x.Items)
            .Create();

        DatabaseContext.Set<Cart>().AddRange(_cart, _irrelevantCart);
        DatabaseContext.SaveChanges();
    }

    [Test]
    public async Task OrdinaryCase_ShouldUpdateCart()
    {
        // Arrange
        var updatedCart = Fixture.Build<Cart>()
            .With(x => x.Id, _cart.Id)
            .Without(x => x.Items)
            .Create();

        DatabaseContext.ChangeTracker.Clear();

        // Act
        await GetService().UpdateAsync(updatedCart, CancellationToken.None);

        // Assert
        var result = await DatabaseContext.Set<Cart>().FindAsync(_cart.Id);
        result.Should().BeEquivalentTo(updatedCart, options => options.Excluding(x => x.Items));

        var untouched = await DatabaseContext.Set<Cart>().FindAsync(_irrelevantCart.Id);
        untouched.Should().BeEquivalentTo(_irrelevantCart);
        
        AssertResponse(updatedCart, result);
    }
}
