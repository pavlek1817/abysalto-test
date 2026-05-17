using System.Text.Json;
using System.Text.Json.Serialization;
using CartService.Application.Infrastructure.Persistence;
using CartService.Domain.Entities;
using CartService.Infrastructure;
using CartService.Infrastructure.Configs;
using CartService.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace CartService.UnitTests.Infrastructure.Persistence.Repositories.CartRepositoryTests;

internal class CartRepositoryTestBase : RepositoryTestBase<ICartRepository>
{
    protected const int CacheExpirationInMinutes = 10;

    protected List<Cart> Carts { get; set; } = null!;
    protected Cart? CapturedCartAddedInCache { get; set; }

    private Mock<IConnectionMultiplexer> MockedConnectionMultiplexer { get; set; } = null!;
    private IOptions<InfrastructureConfig> Config { get; set; } = null!;

    protected Mock<IDatabase> MockedDatabase { get; private set; } = null!;

    protected override ICartRepository GetService()
        => new CartRepository(Config, DatabaseContext, MockedConnectionMultiplexer.Object);

    protected override void InstantiatedDependencies()
    {
        base.InstantiatedDependencies();
        MockedDatabase = new Mock<IDatabase>();
        MockedConnectionMultiplexer = new Mock<IConnectionMultiplexer>();

        MockedConnectionMultiplexer
            .Setup(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<string>()))
            .Returns(MockedDatabase.Object);

        Config = Options.Create(new InfrastructureConfig
        {
            Cache = new RedisConfig { CacheExpirationInMinutes = CacheExpirationInMinutes },
            Database = new PostgreeConfig()
        });
    }

    protected void AssertResponse(Cart expected, Cart? actual)
    {
        actual.Should().NotBeNull();
        actual.Should().BeEquivalentTo(expected,
            options => options.Excluding(x => x.Items));

        actual!.Items.Should().BeEquivalentTo(expected.Items,
            options => options.Excluding(x => x.Cart));
    }

    protected void CaptureCartAddedToCache(string key)
    {
        MockedDatabase.Setup(x => x.StringSetAsync(
                It.Is<RedisKey>(k => k == key),
                It.IsAny<RedisValue>(),
                It.Is<TimeSpan?>(t => t == TimeSpan.FromMinutes(CacheExpirationInMinutes)),
                It.IsAny<bool>(),
                It.IsAny<When>(),
                It.IsAny<CommandFlags>()))
            .Callback((RedisKey _, RedisValue value, TimeSpan? _, bool _, When _, CommandFlags _) =>
            {
                CapturedCartAddedInCache = JsonSerializer.Deserialize<Cart>(value!, new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
            });
    }
}