namespace BaseTemplate.Domain.Entities;

/// <summary>
/// Fallback values used both to seed the initial <see cref="BrandingSettings"/>
/// row and to answer branding queries if that row somehow doesn't exist yet.
/// </summary>
public static class BrandingDefaults
{
    public const string AppName = "BaseTemplate";
    public const string LogoUrl = "assets/logo.svg";
    public const string PrimaryColor = "#4F46E5";
    public const string SecondaryColor = "#F59E0B";
}
