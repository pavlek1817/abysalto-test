using CartService.Application.Features.AddToCart;
using CartService.Application.Infrastructure.External;
using CartService.Domain.Entities;

namespace CartService.UnitTests.Application.Features.AddToCartTests;

internal class OrdinaryCaseTests : AddToCartTestBase
{
    private AddToCartCommand _addToCartCommand;
    
    [SetUp]
    public void SetUp()
    {
        _addToCartCommand = Fixture.Create<AddToCartCommand>();
        
        InstantiatedDependencies();
    }
    
    [Test]
    public async Task CartDoesNotExistCase_ShouldInsertCartAndReturnIt()
    {
        // Arrange
        const int unitPrice = 5;
        MockedProductService
            .Setup(repo => repo.ResolveProductPriceAndSubtractAsync(
                It.Is<ProductModel>(p => p.ProductId == _addToCartCommand.ProductId
                                         && p.Quantity == _addToCartCommand.Quantity), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => unitPrice);
        
        MockedCartRepository.Setup(x => x.GetByOwnerIdAsync(_addToCartCommand.OwnerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => null);
        
        Cart? capturedInsertedCart = null;
        MockedCartRepository.Setup(x => x.InsertAsync(It.IsAny<Cart>(), It.IsAny<CancellationToken>()))
            .Callback((Cart cart, CancellationToken _) =>
            {
                capturedInsertedCart = cart;
            })
            .ReturnsAsync((Cart cart, CancellationToken _) => cart);

        var response = await GetService()
            .HandleAsync(_addToCartCommand, CancellationToken.None);
        
        // Act && Assert
        response.Should().NotBeNull();
        capturedInsertedCart.Should().NotBeNull();
        capturedInsertedCart.Items.Should().HaveCount(1);
        capturedInsertedCart.OwnerId.Should().Be(_addToCartCommand.OwnerId);
        capturedInsertedCart.UpdatedAt.Should().BeNull();
        
        capturedInsertedCart.Items[0].Quantity.Should().Be(_addToCartCommand.Quantity);
        capturedInsertedCart.Items[0].ProductId.Should().Be(_addToCartCommand.ProductId);
        capturedInsertedCart.Items[0].UnitPrice.Should().Be(unitPrice);
        capturedInsertedCart.Items[0].UpdatedAt.Should().BeNull();
        
        response.Id.Should().Be(capturedInsertedCart.Id);
        response.OwnerId.Should().Be(_addToCartCommand.OwnerId);
        response.Items.Should().HaveCount(1);
        response.Items[0].Quantity.Should().Be(_addToCartCommand.Quantity);
        response.Items[0].ProductId.Should().Be(_addToCartCommand.ProductId);
        response.Items[0].UnitPrice.Should().Be(unitPrice);
        response.Items[0].TotalPrice.Should().Be(unitPrice * _addToCartCommand.Quantity);
        
        
        response.CreatedAt.Should().Be(capturedInsertedCart!.CreatedAt);
        

        MockedProductService
            .Verify(repo => repo.ResolveProductPriceAndSubtractAsync(
                    It.Is<ProductModel>(p => p.ProductId == _addToCartCommand.ProductId
                                             && p.Quantity == _addToCartCommand.Quantity),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        
        MockedProductService.VerifyNoOtherCalls();
    }

    [Test]
    public async Task CartExistCase_ShouldUpdateCartAndReturnIt()
    {
        // Arrange
        const int unitPrice = 5;
        const int initialQuantity = 1;
        MockedProductService
            .Setup(repo => repo.ResolveProductPriceAndSubtractAsync(
                It.Is<ProductModel>(p => p.ProductId == _addToCartCommand.ProductId
                                         && p.Quantity == _addToCartCommand.Quantity), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => unitPrice);

        var existingCart = Fixture.Build<Cart>()
            .With(x => x.OwnerId, _addToCartCommand.OwnerId)
            .With(x => x.Items, new List<CartItem>
                {
                    Fixture.Build<CartItem>()
                        .Without(x => x.Cart)
                        .Create(),
                    Fixture.Build<CartItem>()
                        .With(x => x.ProductId, _addToCartCommand.ProductId)
                        .With(x => x.Quantity, initialQuantity)
                        .With(x => x.UnitPrice, unitPrice)
                        .Without(x => x.Cart)
                        .Create(),
                }
            ).Create();
        
        MockedCartRepository.Setup(x => x.GetByOwnerIdAsync(_addToCartCommand.OwnerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => existingCart);
        
        Cart? capturedUpdatedCart = null;
        MockedCartRepository.Setup(x => x.UpdateAsync(It.IsAny<Cart>(), It.IsAny<CancellationToken>()))
            .Callback((Cart cart, CancellationToken _) =>
            {
                capturedUpdatedCart = cart;
            });

        var timestampBefore = DateTime.UtcNow;
        var response = await GetService()
            .HandleAsync(_addToCartCommand, CancellationToken.None);
        var timestampAfter = DateTime.UtcNow;
        
        // Act && Assert
        response.Should().NotBeNull();
        capturedUpdatedCart.Should().NotBeNull();
        capturedUpdatedCart.Items.Should().HaveCount(2);
        capturedUpdatedCart.OwnerId.Should().Be(_addToCartCommand.OwnerId);
        capturedUpdatedCart.UpdatedAt.Should().NotBeNull();
        capturedUpdatedCart.UpdatedAt.Should().NotBeBefore(timestampBefore)
            .And.NotBeAfter(timestampAfter);
        
        capturedUpdatedCart.Items[1].Quantity.Should().Be(_addToCartCommand.Quantity + initialQuantity);
        capturedUpdatedCart.Items[1].ProductId.Should().Be(_addToCartCommand.ProductId);
        capturedUpdatedCart.Items[1].UnitPrice.Should().Be(unitPrice);
        capturedUpdatedCart.Items[1].UpdatedAt.Should().NotBeNull();
        capturedUpdatedCart.Items[1].UpdatedAt.Should().NotBeBefore(timestampBefore)
            .And.NotBeAfter(timestampAfter);
        
        response.Id.Should().Be(capturedUpdatedCart.Id);
        response.OwnerId.Should().Be(_addToCartCommand.OwnerId);
        response.Items.Should().HaveCount(2);
        response.Items[1].Quantity.Should().Be(_addToCartCommand.Quantity + initialQuantity);
        response.Items[1].ProductId.Should().Be(_addToCartCommand.ProductId);
        response.Items[1].UnitPrice.Should().Be(unitPrice);
        response.Items[1].TotalPrice.Should().Be(unitPrice * (_addToCartCommand.Quantity + initialQuantity));
        
        
        response.CreatedAt.Should().Be(capturedUpdatedCart!.CreatedAt);
        

        MockedProductService
            .Verify(repo => repo.ResolveProductPriceAndSubtractAsync(
                    It.Is<ProductModel>(p => p.ProductId == _addToCartCommand.ProductId
                                             && p.Quantity == _addToCartCommand.Quantity),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        
        MockedProductService.VerifyNoOtherCalls();
    }
}