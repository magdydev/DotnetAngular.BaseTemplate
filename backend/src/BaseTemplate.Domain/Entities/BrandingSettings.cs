using System.Text.RegularExpressions;
using BaseTemplate.Domain.Common;
using BaseTemplate.Domain.Events;
using BaseTemplate.Domain.Exceptions;

namespace BaseTemplate.Domain.Entities;

/// <summary>
/// Global, single-row settings that drive the frontend's name/logo/colors.
/// There is exactly one of these — see <c>IBrandingSettingsRepository.GetCurrentAsync</c>
/// — it's a settings aggregate, not a per-user or per-tenant one.
/// </summary>
public sealed partial class BrandingSettings : BaseEntity, IAggregateRoot
{
    public string AppName { get; private set; } = null!;

    public string? LogoUrl { get; private set; }

    public string PrimaryColor { get; private set; } = null!;

    public string SecondaryColor { get; private set; } = null!;

    private BrandingSettings()
    {
        // Required by EF Core.
    }

    public static BrandingSettings CreateDefault(string appName, string? logoUrl, string primaryColor, string secondaryColor)
    {
        var settings = new BrandingSettings();
        settings.Update(appName, logoUrl, primaryColor, secondaryColor);
        return settings;
    }

    public void Update(string appName, string? logoUrl, string primaryColor, string secondaryColor)
    {
        if (string.IsNullOrWhiteSpace(appName))
        {
            throw new DomainException("Application name is required.");
        }

        if (!IsValidHexColor(primaryColor))
        {
            throw new DomainException($"'{primaryColor}' is not a valid hex color.");
        }

        if (!IsValidHexColor(secondaryColor))
        {
            throw new DomainException($"'{secondaryColor}' is not a valid hex color.");
        }

        var trimmedLogoUrl = string.IsNullOrWhiteSpace(logoUrl) ? null : logoUrl.Trim();
        var changed = AppName != appName || LogoUrl != trimmedLogoUrl || PrimaryColor != primaryColor || SecondaryColor != secondaryColor;

        AppName = appName.Trim();
        LogoUrl = trimmedLogoUrl;
        PrimaryColor = primaryColor.Trim();
        SecondaryColor = secondaryColor.Trim();

        if (changed)
        {
            AddDomainEvent(new BrandingSettingsUpdatedDomainEvent(Id, AppName));
        }
    }

    private static bool IsValidHexColor(string? value) =>
        !string.IsNullOrWhiteSpace(value) && HexColorRegex().IsMatch(value);

    [GeneratedRegex("^#([0-9A-Fa-f]{6}|[0-9A-Fa-f]{3})$")]
    private static partial Regex HexColorRegex();
}
