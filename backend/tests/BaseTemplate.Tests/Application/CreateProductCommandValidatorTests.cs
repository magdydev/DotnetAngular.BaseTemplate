using BaseTemplate.Application.Products.Commands.CreateProduct;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace BaseTemplate.Tests.Application;

public class CreateProductCommandValidatorTests
{
    private readonly CreateProductCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_HasNoErrors()
    {
        var command = new CreateProductCommand("Widget", "WGT-1", 19.99m, "USD", "A sample widget");

        var result = _validator.TestValidate(command);

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("", "WGT-1", 10, "USD")]
    [InlineData("Widget", "", 10, "USD")]
    [InlineData("Widget", "WGT-1", -1, "USD")]
    [InlineData("Widget", "WGT-1", 10, "US")]
    public void Validate_WithInvalidCommand_HasErrors(string name, string sku, decimal price, string currency)
    {
        var command = new CreateProductCommand(name, sku, price, currency, null);

        var result = _validator.TestValidate(command);

        result.IsValid.Should().BeFalse();
    }
}
