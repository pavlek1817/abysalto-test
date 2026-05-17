using CartService.Domain.Entities;
using StackExchange.Redis;

namespace CartService.UnitTests.Infrastructure.Persistence.Repositories.CartRepositoryTests;

internal class DeleteAsyncTests : CartRepositoryTestBase
{
    private Cart _cart = null!;
    private Cart _irrelevantCart = null!;

    [SetUp]
    public void SetUp()
    {
        InstantiatedDependencies();

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
    public async Task OrdinaryCase_ShouldDeleteCart()
    {
        // Act
        await GetService().DeleteAsync(_cart.Id, CancellationToken.None);

        // Assert
        var result = await DatabaseContext.Set<Cart>().FindAsync(_cart.Id);
        result.Should().BeNull();

        var untouched = await DatabaseContext.Set<Cart>().FindAsync(_irrelevantCart.Id);
        untouched.Should().BeEquivalentTo(_irrelevantCart);

        MockedDatabase.Verify(x => x.KeyDeleteAsync(
            It.Is<RedisKey>(k => k == $"cart:{_cart.OwnerId}"),
            It.IsAny<CommandFlags>()), Times.Once);
    }
}
