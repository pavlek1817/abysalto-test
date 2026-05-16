using CartService.Domain.Entities;

namespace CartService.UnitTests.Infrastructure.Persistence.Repositories.CartRepositoryTests;

internal class DeleteAsyncTests : CartRepositoryTestBase
{
    private Cart _cart = null!;
    private Cart _irrelevantCart = null!;

    [SetUp]
    public void SetUp()
    {
        this.InstantiatedDependencies();

        this._cart = this.Fixture.Build<Cart>()
            .Without(x => x.Items)
            .Create();

        this._irrelevantCart = this.Fixture.Build<Cart>()
            .Without(x => x.Items)
            .Create();

        this.DatabaseContext.Set<Cart>().AddRange(_cart, _irrelevantCart);
        this.DatabaseContext.SaveChanges();
    }

    [Test]
    public async Task OrdinaryCase_ShouldDeleteCart()
    {
        // Act
        await this.GetService().DeleteAsync(_cart.Id, CancellationToken.None);

        // Assert
        var result = await this.DatabaseContext.Set<Cart>().FindAsync(_cart.Id);
        result.Should().BeNull();

        var untouched = await this.DatabaseContext.Set<Cart>().FindAsync(_irrelevantCart.Id);
        untouched.Should().BeEquivalentTo(_irrelevantCart);
    }
}
