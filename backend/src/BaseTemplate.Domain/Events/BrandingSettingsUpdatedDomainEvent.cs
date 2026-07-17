using BaseTemplate.Domain.Common;

namespace BaseTemplate.Domain.Events;

public sealed record BrandingSettingsUpdatedDomainEvent(Guid SettingsId, string AppName) : DomainEvent;
