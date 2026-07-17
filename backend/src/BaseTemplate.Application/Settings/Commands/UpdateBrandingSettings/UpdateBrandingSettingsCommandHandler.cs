using AutoMapper;
using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Application.Settings.Dtos;
using BaseTemplate.Domain.Entities;
using BaseTemplate.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace BaseTemplate.Application.Settings.Commands.UpdateBrandingSettings;

public sealed class UpdateBrandingSettingsCommandHandler(
    IUnitOfWork unitOfWork,
    IDomainEventDispatcher domainEventDispatcher,
    IMapper mapper,
    ILogger<UpdateBrandingSettingsCommandHandler> logger)
    : ICommandHandler<UpdateBrandingSettingsCommand, BrandingSettingsDto>
{
    public async Task<BrandingSettingsDto> Handle(UpdateBrandingSettingsCommand command, CancellationToken cancellationToken)
    {
        var settings = await unitOfWork.BrandingSettings.GetCurrentAsync(cancellationToken);

        if (settings is null)
        {
            settings = BrandingSettings.CreateDefault(command.AppName, command.LogoUrl, command.PrimaryColor, command.SecondaryColor);
            await unitOfWork.BrandingSettings.AddAsync(settings, cancellationToken);
        }
        else
        {
            settings.Update(command.AppName, command.LogoUrl, command.PrimaryColor, command.SecondaryColor);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        await domainEventDispatcher.DispatchAndClearEvents([settings], cancellationToken);

        logger.LogInformation("Branding settings updated: {AppName}", settings.AppName);

        return mapper.Map<BrandingSettingsDto>(settings);
    }
}
