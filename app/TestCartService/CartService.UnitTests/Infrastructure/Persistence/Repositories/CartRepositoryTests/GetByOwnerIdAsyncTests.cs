using System.Text.Json;
using System.Text.Json.Serialization;
using CartService.Domain.Entities;
using StackExchange.Redis;

namespace CartService.UnitTests.Infrastructure.Persistence.Repositories.CartRepositoryTests;

internal class GetByOwnerIdAsyncTests : CartRepositoryTestBase
{
    private int _cartId;
    private string _ownerId = string.Empty;

    [SetUp]
    public void SetUp()
    {
        _cartId = Fixture.Create<int>();
        _ownerId = Fixture.Create<string>();

        InstantiatedDependencies();

        CaptureCartAddedToCache();

        Carts = new List<Cart>
        {
            new()
            {
                Id = _cartId,
                OwnerId = _ownerId,
                CreatedAt = DateTime.UtcNow,
                Items =
                [
                    Fixture.Build<CartItem>()
                        .With(x => x.CartId, _cartId)
                        .Without(x => x.Cart)
                        .Create(),
                    Fixture.Build<CartItem>()
                        .With(x => x.CartId, _cartId)
                        .Without(x => x.Cart)
                        .Create()
                ]
            },
            Fixture.Build<Cart>()
                .Without(x => x.Items)
                .Create()
        };

        DatabaseContext.Set<Cart>().AddRange(Carts);
        DatabaseContext.SaveChanges();
    }

    [Test]
    public async Task CartExistOnlyInDatabaseCase_ShouldReturnCorrectObject_AndAddToCache()
    {
        // Act
        var result = await GetService().GetByOwnerIdAsync(_ownerId, CancellationToken.None);

        // Assert
        var expected = Carts.Single(x => x.OwnerId == _ownerId);
        AssertResponse(expected, result);
        AssertResponse(expected, CapturedCartAddedInCache);
    }

    [Test]
    public async Task CartExistInCache_ShouldReturnCartFromCache()
    {
        // Arrange
        var jsonOptions = new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.IgnoreCycles };
        MockedDatabase.Setup(x => x.StringGetAsync(
            It.IsAny<RedisKey>(),
            It.IsAny<CommandFlags>()))
            .ReturnsAsync(() =>
                new RedisValue(JsonSerializer.Serialize(
                    this.Carts.Single(x => x.OwnerId == _ownerId), jsonOptions)));

        // Act
        var result = await GetService().GetByOwnerIdAsync(_ownerId, CancellationToken.None);

        // Assert
        var expected = Carts.Single(x => x.OwnerId == _ownerId);
        AssertResponse(expected, result!);

        MockedDatabase.Verify(x => x.StringGetAsync(
            It.Is<RedisKey>(k => k == $"cart:{_ownerId}"),
            It.IsAny<CommandFlags>()), Times.Once);
    }

    [Test]
    public async Task CartDoesNotExist_ShouldReturnNull()
    {
        // Act
        var result = await GetService().GetByOwnerIdAsync(Fixture.Create<string>(), CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }
}