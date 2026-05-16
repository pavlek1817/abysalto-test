using CartService.Application.Persistence;
using CartService.Infrastructure.Persistence.Repositories;

namespace CartService.UnitTests.Infrastructure.Repositories.CartRepositoryTests;

internal class CartRepositoryTestBase : RepositoryTestBase<ICartRepository>
{
    protected override ICartRepository GetService()
        => new CartRepository(this.DatabaseContext);
}