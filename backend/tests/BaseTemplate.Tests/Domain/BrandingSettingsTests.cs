using BaseTemplate.Domain.Entities;
using BaseTemplate.Domain.Events;
using BaseTemplate.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace BaseTemplate.Tests.Domain;

public class BrandingSettingsTests
{
    [Fact]
    public void CreateDefault_WithValidData_RaisesUpdatedDomainEvent()
    {
        var settings = BrandingSettings.CreateDefault("Acme", "assets/logo.svg", "#4F46E5", "#F59E0B");

        settings.AppName.Should().Be("Acme");
        settings.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<BrandingSettingsUpdatedDomainEvent>();
    }

    [Theory]
    [InlineData("4F46E5")]
    [InlineData("#4F46E")]
    [InlineData("#GGGGGG")]
    [InlineData("")]
    public void Update_WithInvalidPrimaryColor_ThrowsDomainException(string invalidColor)
    {
        var settings = BrandingSettings.CreateDefault("Acme", null, "#4F46E5", "#F59E0B");

        var act = () => settings.Update("Acme", null, invalidColor, "#F59E0B");

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Update_WithEmptyAppName_ThrowsDomainException()
    {
        var settings = BrandingSettings.CreateDefault("Acme", null, "#4F46E5", "#F59E0B");

        var act = () => settings.Update(string.Empty, null, "#4F46E5", "#F59E0B");

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Update_ToSameValues_DoesNotRaiseEvent()
    {
        var settings = BrandingSettings.CreateDefault("Acme", "assets/logo.svg", "#4F46E5", "#F59E0B");
        settings.ClearDomainEvents();

        settings.Update("Acme", "assets/logo.svg", "#4F46E5", "#F59E0B");

        settings.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void Update_WithShortHexColor_IsAccepted()
    {
        var settings = BrandingSettings.CreateDefault("Acme", null, "#4F46E5", "#F59E0B");

        settings.Update("Acme", null, "#FFF", "#000");

        settings.PrimaryColor.Should().Be("#FFF");
        settings.SecondaryColor.Should().Be("#000");
    }
}
