using AutoMapper;
using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Application.Settings.Dtos;
using BaseTemplate.Domain.Entities;
using BaseTemplate.Domain.Repositories;

namespace BaseTemplate.Application.Settings.Queries.GetBrandingSettings;

public sealed class GetBrandingSettingsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IQueryHandler<GetBrandingSettingsQuery, BrandingSettingsDto>
{
    public async Task<BrandingSettingsDto> Handle(GetBrandingSettingsQuery query, CancellationToken cancellationToken)
    {
        var settings = await unitOfWork.BrandingSettings.GetCurrentAsync(cancellationToken);

        // Falls back to defaults instead of a 404 — the frontend fetches this on
        // every startup and should always get *something* to render, even
        // against a brand-new database whose seed hasn't run yet.
        return settings is null
            ? new BrandingSettingsDto
            {
                AppName = BrandingDefaults.AppName,
                LogoUrl = BrandingDefaults.LogoUrl,
                PrimaryColor = BrandingDefaults.PrimaryColor,
                SecondaryColor = BrandingDefaults.SecondaryColor,
            }
            : mapper.Map<BrandingSettingsDto>(settings);
    }
}
