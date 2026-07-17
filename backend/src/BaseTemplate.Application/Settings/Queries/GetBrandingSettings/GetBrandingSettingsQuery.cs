using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Application.Settings.Dtos;

namespace BaseTemplate.Application.Settings.Queries.GetBrandingSettings;

public sealed record GetBrandingSettingsQuery : IQuery<BrandingSettingsDto>;
