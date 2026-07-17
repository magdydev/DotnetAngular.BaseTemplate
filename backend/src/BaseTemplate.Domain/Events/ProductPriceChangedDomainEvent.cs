using BaseTemplate.Domain.Common;

namespace BaseTemplate.Domain.Events;

public sealed record ProductPriceChangedDomainEvent(Guid ProductId, decimal OldPrice, decimal NewPrice) : DomainEvent;
