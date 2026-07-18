using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Application.Settings.Dtos;

namespace BaseTemplate.Application.Settings.Commands.UpdateBrandingSettings;

public sealed record UpdateBrandingSettingsCommand(
    string AppName,
    string AppNameAr,
    string? LogoUrl,
    string? LogoData,
    string PrimaryColor,
    string SecondaryColor) : ICommand<BrandingSettingsDto>;
