using AutoMapper;
using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Application.Settings.Dtos;
using BaseTemplate.Domain.Entities;
using BaseTemplate.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace BaseTemplate.Application.Settings.Queries.GetBrandingSettings;

public sealed class GetBrandingSettingsQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger<GetBrandingSettingsQueryHandler> logger)
    : IQueryHandler<GetBrandingSettingsQuery, BrandingSettingsDto>
{
    public async Task<BrandingSettingsDto> Handle(GetBrandingSettingsQuery query, CancellationToken cancellationToken)
    {
        var settings = await unitOfWork.BrandingSettings.GetCurrentAsync(cancellationToken);

        if (settings is null)
        {
            logger.LogInformation("No branding settings found — returning defaults");
            return new BrandingSettingsDto
            {
                AppName = BrandingDefaults.AppName,
                AppNameAr = BrandingDefaults.AppNameAr,
                LogoUrl = BrandingDefaults.LogoUrl,
                LogoData = null,
                PrimaryColor = BrandingDefaults.PrimaryColor,
                SecondaryColor = BrandingDefaults.SecondaryColor,
            };
        }

        logger.LogDebug("Returning branding settings for '{AppName}'", settings.AppName);
        return mapper.Map<BrandingSettingsDto>(settings);
    }
}
