using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Domain.Events;
using Microsoft.Extensions.Logging;

namespace BaseTemplate.Application.Products.EventHandlers;

/// <summary>
/// Sample reaction to a domain event. Real handlers might send an email,
/// publish an integration event to a message bus, update a read model, etc.
/// </summary>
public sealed class ProductCreatedDomainEventHandler(ILogger<ProductCreatedDomainEventHandler> logger)
    : IDomainEventHandler<ProductCreatedDomainEvent>
{
    public Task Handle(ProductCreatedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Domain event handled: product {ProductId} ({Name}) was created at {OccurredOn}",
            domainEvent.ProductId,
            domainEvent.Name,
            domainEvent.OccurredOn);

        return Task.CompletedTask;
    }
}
