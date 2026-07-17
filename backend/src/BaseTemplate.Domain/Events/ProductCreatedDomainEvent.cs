using BaseTemplate.Domain.Common;

namespace BaseTemplate.Domain.Events;

public sealed record ProductCreatedDomainEvent(Guid ProductId, string Name) : DomainEvent;
