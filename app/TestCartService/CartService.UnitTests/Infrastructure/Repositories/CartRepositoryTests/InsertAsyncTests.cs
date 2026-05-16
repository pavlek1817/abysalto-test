using CartService.Domain.Entities;
using FluentAssertions;

namespace CartService.UnitTests.Infrastructure.Repositories.CartRepositoryTests;

internal class InsertAsyncTests : CartRepositoryTestBase
{
    private Cart _cart = null!;
    private Cart _existingCart = null!;

    [SetUp]
    public void SetUp()
    {
        this.InstantiatedDependencies();

        this._existingCart = this.Fixture.Build<Cart>()
            .Without(x => x.Items)
            .Create();

        this.DatabaseContext.Set<Cart>().Add(_existingCart);
        this.DatabaseContext.SaveChanges();

        this._cart = this.Fixture.Build<Cart>()
            .Without(x => x.Items)
            .Create();
    }

    [Test]
    public async Task OrdinaryCase_ShouldInsertCart()
    {
        // Act
        var result = await this.GetService().InsertAsync(_cart, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(_cart, options => options.Excluding(x => x.CreatedAt));

        var inserted = await this.DatabaseContext.Set<Cart>().FindAsync(result.Id);
        inserted.Should().NotBeNull();

        var untouched = await this.DatabaseContext.Set<Cart>().FindAsync(_existingCart.Id);
        untouched.Should().BeEquivalentTo(_existingCart);
    }
}
