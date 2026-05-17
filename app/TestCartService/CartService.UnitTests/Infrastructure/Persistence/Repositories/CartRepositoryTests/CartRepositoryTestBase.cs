using System.Text.Json;
using System.Text.Json.Serialization;
using CartService.Application.Infrastructure.Persistence;
using CartService.Domain.Entities;
using CartService.Infrastructure.Persistence.Repositories;
using StackExchange.Redis;

namespace CartService.UnitTests.Infrastructure.Persistence.Repositories.CartRepositoryTests;

internal class CartRepositoryTestBase : RepositoryTestBase<ICartRepository>
{
    protected List<Cart> Carts { get; set; } = null!;
    protected Cart? CapturedCartAddedInCache { get; set; } = null;
    
    private Mock<IConnectionMultiplexer>
        MockedConnectionMultiplexer { get; set; } = null!;
    
    protected Mock<IDatabase> MockedDatabase { get; private set; } = null!;
    
    protected override ICartRepository GetService()
        => new CartRepository(DatabaseContext,  MockedConnectionMultiplexer.Object);

    protected override void InstantiatedDependencies()
    {
        base.InstantiatedDependencies();
        MockedDatabase = new Mock<IDatabase>();
        MockedConnectionMultiplexer = new Mock<IConnectionMultiplexer>();
        
        MockedConnectionMultiplexer
            .Setup(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<string>()))
            .Returns(MockedDatabase.Object);
    }
    
    protected void AssertResponse(Cart expected, Cart? actual)
    {
        actual.Should().NotBeNull();
        actual.Should().BeEquivalentTo(expected, 
            options => options.Excluding(x => x.Items));
        
        actual!.Items.Should().BeEquivalentTo(expected.Items, 
            options => options.Excluding(x => x.Cart));
    }

    protected void CaptureCartAddedToCache()
    {
        MockedDatabase.Setup(x => x.StringSetAsync(
                It.IsAny<RedisKey>(),
                It.IsAny<RedisValue>(),
                It.IsAny<TimeSpan?>(),
                It.IsAny<bool>(),
                It.IsAny<When>(),
                It.IsAny<CommandFlags>()))
            .Callback((RedisKey key, RedisValue value, TimeSpan? expiry, bool keepTtl, When when, CommandFlags flags) =>
            {
                if (key.ToString().StartsWith("cart:"))
                {
                    CapturedCartAddedInCache = JsonSerializer.Deserialize<Cart>(value!, new JsonSerializerOptions
                    {
                        ReferenceHandler = ReferenceHandler.IgnoreCycles
                    });
                }
            });
    }
}