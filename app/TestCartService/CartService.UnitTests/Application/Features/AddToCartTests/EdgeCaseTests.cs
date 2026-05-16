using CartService.Application.Exceptions;
using CartService.Application.Features.AddToCart;
using CartService.Application.Infrastructure.External;
using CartService.Domain.Entities;

namespace CartService.UnitTests.Application.Features.AddToCartTests;

internal class EdgeCaseTests : AddToCartTestBase
{
    private AddToCartCommand _addToCartCommand;
    
    [SetUp]
    public void SetUp()
    {
        this._addToCartCommand = this.Fixture.Create<AddToCartCommand>();
        
        this.InstantiatedDependencies();
    }

    [TestCase(typeof(NotEnoughQuantityException))]
    [TestCase(typeof(ItemDoesNotExistException))]
    public void ProductServiceResolveCallThrowsExceptionCase_ShouldReThrowException(Type exceptionType)
    {
        // Arrange
        var errorMessage = this.Fixture.Create<string>();
        var constructor = exceptionType.GetConstructor(new[] { typeof(string) });
        var exception = (Exception)constructor!.Invoke([errorMessage]);

        this.MockedProductService
            .Setup(repo => repo.ResolveProductPriceAndSubtractAsync(
                It.Is<ProductModel>(p => p.ProductId == _addToCartCommand.ProductId
                && p.Quantity == _addToCartCommand.Quantity), It.IsAny<CancellationToken>()))
            .Throws(() => exception);

        var act = () => this.GetService()
            .HandleAsync(this._addToCartCommand, CancellationToken.None);

        // Act & Assert
        Assert.ThrowsAsync(exceptionType, () => act.Invoke());
        
        this.MockedCartRepository.VerifyNoOtherCalls();

        this.MockedProductService
            .Verify(repo => repo.ResolveProductPriceAndSubtractAsync(
                It.IsAny<ProductModel>(), It.IsAny<CancellationToken>()), Times.Once);
        
        this.MockedProductService.VerifyNoOtherCalls();
    }

    [Test]
    public async Task InsertInDatabaseFailCase_ShouldReplenishTheProduct()
    {
        // Arrange
        const int unitPrice = 5;
        this.MockedProductService
            .Setup(repo => repo.ResolveProductPriceAndSubtractAsync(
                It.Is<ProductModel>(p => p.ProductId == _addToCartCommand.ProductId
                                         && p.Quantity == _addToCartCommand.Quantity), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => unitPrice);
        
        this.MockedCartRepository.Setup(x => x.GetByOwnerIdAsync(_addToCartCommand.OwnerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => null);
        
        this.MockedCartRepository.Setup(x => x.InsertAsync(It.IsAny<Domain.Entities.Cart>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());
        
        
        var act = () => this.GetService()
            .HandleAsync(this._addToCartCommand, CancellationToken.None);
        
        // Act && Assert
        await act.Should().ThrowExactlyAsync<Exception>();

        this.MockedProductService
            .Verify(repo => repo.ResolveProductPriceAndSubtractAsync(
                    It.Is<ProductModel>(p => p.ProductId == _addToCartCommand.ProductId
                                             && p.Quantity == _addToCartCommand.Quantity),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        
        this.MockedProductService.Verify(x => x.ReplenishProductStockAsync(
            It.Is<ProductModel>(p => p.ProductId == _addToCartCommand.ProductId
                                     && p.Quantity == _addToCartCommand.Quantity), It.IsAny<CancellationToken>()), Times.Once);
        
        this.MockedProductService.VerifyNoOtherCalls();
    }

    [Test]
    public async Task UpdateInDatabaseFailCase_ShouldReplenishTheProduct()
    {
        // Arrange
        const int unitPrice = 5;
        this.MockedProductService
            .Setup(repo => repo.ResolveProductPriceAndSubtractAsync(
                It.Is<ProductModel>(p => p.ProductId == _addToCartCommand.ProductId
                                         && p.Quantity == _addToCartCommand.Quantity), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => unitPrice);
        
        this.MockedCartRepository.Setup(x => x.GetByOwnerIdAsync(_addToCartCommand.OwnerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => this.Fixture.Build<Cart>()
                .Without(x => x.Items)
                .Create()
            );
        
        this.MockedCartRepository.Setup(x => x.UpdateAsync(It.IsAny<Domain.Entities.Cart>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());
        
        
        var act = () => this.GetService()
            .HandleAsync(this._addToCartCommand, CancellationToken.None);
        
        // Act && Assert
        await act.Should().ThrowExactlyAsync<Exception>();

        this.MockedProductService
            .Verify(repo => repo.ResolveProductPriceAndSubtractAsync(
                    It.Is<ProductModel>(p => p.ProductId == _addToCartCommand.ProductId
                                             && p.Quantity == _addToCartCommand.Quantity),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        
        this.MockedProductService.Verify(x => x.ReplenishProductStockAsync(
            It.Is<ProductModel>(p => p.ProductId == _addToCartCommand.ProductId
                                     && p.Quantity == _addToCartCommand.Quantity), It.IsAny<CancellationToken>()), Times.Once);
        
        this.MockedProductService.VerifyNoOtherCalls();
    }
}