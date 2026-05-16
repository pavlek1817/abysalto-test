using CartService.Application.Infrastructure.Persistence;
using CartService.Infrastructure.Persistence.Repositories;

namespace CartService.UnitTests.Infrastructure.Persistence.Repositories.CartRepositoryTests;

internal class CartRepositoryTestBase : RepositoryTestBase<ICartRepository>
{
    protected override ICartRepository GetService()
        => new CartRepository(this.DatabaseContext);
}