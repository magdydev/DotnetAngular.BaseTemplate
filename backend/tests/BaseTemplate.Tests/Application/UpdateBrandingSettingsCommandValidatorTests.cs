using BaseTemplate.Application.Settings.Commands.UpdateBrandingSettings;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace BaseTemplate.Tests.Application;

public class UpdateBrandingSettingsCommandValidatorTests
{
    private readonly UpdateBrandingSettingsCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_HasNoErrors()
    {
        var command = new UpdateBrandingSettingsCommand("Acme", "assets/logo.svg", "#4F46E5", "#F59E0B");

        var result = _validator.TestValidate(command);

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("", "#4F46E5", "#F59E0B")]
    [InlineData("Acme", "4F46E5", "#F59E0B")]
    [InlineData("Acme", "#4F46E5", "not-a-color")]
    public void Validate_WithInvalidCommand_HasErrors(string appName, string primaryColor, string secondaryColor)
    {
        var command = new UpdateBrandingSettingsCommand(appName, null, primaryColor, secondaryColor);

        var result = _validator.TestValidate(command);

        result.IsValid.Should().BeFalse();
    }
}
