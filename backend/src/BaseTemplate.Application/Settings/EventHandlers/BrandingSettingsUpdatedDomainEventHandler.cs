using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Domain.Events;
using Microsoft.Extensions.Logging;

namespace BaseTemplate.Application.Settings.EventHandlers;

public sealed class BrandingSettingsUpdatedDomainEventHandler(ILogger<BrandingSettingsUpdatedDomainEventHandler> logger)
    : IDomainEventHandler<BrandingSettingsUpdatedDomainEvent>
{
    public Task Handle(BrandingSettingsUpdatedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Domain event handled: branding settings updated to '{AppName}' at {OccurredOn}",
            domainEvent.AppName,
            domainEvent.OccurredOn);

        return Task.CompletedTask;
    }
}
