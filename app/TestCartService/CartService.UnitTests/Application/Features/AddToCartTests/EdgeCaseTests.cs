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
        _addToCartCommand = Fixture.Create<AddToCartCommand>();
        
        InstantiatedDependencies();
    }

    [TestCase(typeof(NotEnoughQuantityException))]
    [TestCase(typeof(ItemDoesNotExistException))]
    public void ProductServiceResolveCallThrowsExceptionCase_ShouldReThrowException(Type exceptionType)
    {
        // Arrange
        var errorMessage = Fixture.Create<string>();
        var constructor = exceptionType.GetConstructor(new[] { typeof(string) });
        var exception = (Exception)constructor!.Invoke([errorMessage]);

        MockedProductService
            .Setup(repo => repo.ResolveProductPriceAndSubtractAsync(
                It.Is<ProductModel>(p => p.ProductId == _addToCartCommand.ProductId
                && p.Quantity == _addToCartCommand.Quantity), It.IsAny<CancellationToken>()))
            .Throws(() => exception);

        var act = () => GetService()
            .HandleAsync(_addToCartCommand, CancellationToken.None);

        // Act & Assert
        Assert.ThrowsAsync(exceptionType, () => act.Invoke());
        
        MockedCartRepository.VerifyNoOtherCalls();

        MockedProductService
            .Verify(repo => repo.ResolveProductPriceAndSubtractAsync(
                It.IsAny<ProductModel>(), It.IsAny<CancellationToken>()), Times.Once);
        
        MockedProductService.VerifyNoOtherCalls();
    }

    [Test]
    public async Task InsertInDatabaseFailCase_ShouldReplenishTheProduct()
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
        
        MockedCartRepository.Setup(x => x.InsertAsync(It.IsAny<Cart>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());
        
        
        var act = () => GetService()
            .HandleAsync(_addToCartCommand, CancellationToken.None);
        
        // Act && Assert
        await act.Should().ThrowExactlyAsync<Exception>();

        MockedProductService
            .Verify(repo => repo.ResolveProductPriceAndSubtractAsync(
                    It.Is<ProductModel>(p => p.ProductId == _addToCartCommand.ProductId
                                             && p.Quantity == _addToCartCommand.Quantity),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        
        MockedProductService.Verify(x => x.ReplenishProductStockAsync(
            It.Is<ProductModel>(p => p.ProductId == _addToCartCommand.ProductId
                                     && p.Quantity == _addToCartCommand.Quantity), It.IsAny<CancellationToken>()), Times.Once);
        
        MockedProductService.VerifyNoOtherCalls();
    }

    [Test]
    public async Task UpdateInDatabaseFailCase_ShouldReplenishTheProduct()
    {
        // Arrange
        const int unitPrice = 5;
        MockedProductService
            .Setup(repo => repo.ResolveProductPriceAndSubtractAsync(
                It.Is<ProductModel>(p => p.ProductId == _addToCartCommand.ProductId
                                         && p.Quantity == _addToCartCommand.Quantity), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => unitPrice);
        
        MockedCartRepository.Setup(x => x.GetByOwnerIdAsync(_addToCartCommand.OwnerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => Fixture.Build<Cart>()
                .Without(x => x.Items)
                .Create()
            );
        
        MockedCartRepository.Setup(x => x.UpdateAsync(It.IsAny<Cart>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());
        
        
        var act = () => GetService()
            .HandleAsync(_addToCartCommand, CancellationToken.None);
        
        // Act && Assert
        await act.Should().ThrowExactlyAsync<Exception>();

        MockedProductService
            .Verify(repo => repo.ResolveProductPriceAndSubtractAsync(
                    It.Is<ProductModel>(p => p.ProductId == _addToCartCommand.ProductId
                                             && p.Quantity == _addToCartCommand.Quantity),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        
        MockedProductService.Verify(x => x.ReplenishProductStockAsync(
            It.Is<ProductModel>(p => p.ProductId == _addToCartCommand.ProductId
                                     && p.Quantity == _addToCartCommand.Quantity), It.IsAny<CancellationToken>()), Times.Once);
        
        MockedProductService.VerifyNoOtherCalls();
    }
}