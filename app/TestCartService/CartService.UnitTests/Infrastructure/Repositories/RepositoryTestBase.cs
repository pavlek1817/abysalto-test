using CartService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CartService.UnitTests.Infrastructure.Repositories;

internal abstract class RepositoryTestBase<TRepository> : TestBase
    where TRepository : class
{
    protected CartDbContext DatabaseContext { get; private set; } = null!;

    /// <summary>
    /// Method that gets an instance of service.
    /// </summary>
    protected abstract TRepository GetService();

    /// <summary>
    /// Method that creates instances for every necessary dependency.
    /// </summary>
    protected virtual void InstantiatedDependencies()
    {
        var options = new DbContextOptionsBuilder<CartDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        DatabaseContext = new CartDbContext(options);
    }
}