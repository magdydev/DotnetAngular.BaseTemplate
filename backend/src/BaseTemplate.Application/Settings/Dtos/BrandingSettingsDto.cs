namespace BaseTemplate.Application.Settings.Dtos;

public sealed class BrandingSettingsDto
{
    public string AppName { get; init; } = string.Empty;

    public string AppNameAr { get; init; } = string.Empty;

    public string? LogoUrl { get; init; }

    public string? LogoData { get; init; }

    public string PrimaryColor { get; init; } = string.Empty;

    public string SecondaryColor { get; init; } = string.Empty;
}
