using CartService.Api.Validators;
using CartService.Application.Features.AddToCart;

namespace CartService.UnitTests.Api.Validators;

internal class AddToCartCommandValidatorTests : TestBase
{
    private AddToCartCommandValidator validator = new ();

    private string validOwnerId = "Some-id";
    private int validProductId = 1;
    private int validQuantity = 1;

    [Test]
    public async Task ValidCase_ShouldReturnTrue()
    {
        // Assert
        var command = new AddToCartCommand(validOwnerId, validProductId, validQuantity);
        
        // Act
        var validationResult = await validator.ValidateAsync(command, CancellationToken.None);
        
        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeTrue();
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase(" ")]
    public async Task InvalidOwnerId_ShouldReturnFalse(string? invalidOwnerId)
    {
        // Arrange
        var command = new AddToCartCommand(invalidOwnerId, validProductId, validQuantity);
        
        // Act
        var validationResult = await validator.ValidateAsync(command, CancellationToken.None);
        
        // Assert
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().NotBeEmpty();
    }

    [TestCase(-5)]
    [TestCase(0)]
    public async Task InvalidProductId_ShouldReturnFalse(int invalidProductId)
    {
        // Arrange
        var command = new AddToCartCommand(validOwnerId, invalidProductId, validQuantity);
        
        // Act
        var validationResult = await validator.ValidateAsync(command, CancellationToken.None);
        
        // Assert
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().NotBeEmpty();
    }
    
    [TestCase(-5)]
    [TestCase(0)]
    public async Task InvalidQuantity_ShouldReturnFalse(int invalidQuantity)
    {
        // Arrange
        var command = new AddToCartCommand(validOwnerId, validProductId, invalidQuantity);
        
        // Act
        var validationResult = await validator.ValidateAsync(command, CancellationToken.None);
        
        // Assert
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().NotBeEmpty();
    }
}