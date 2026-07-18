using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Domain.Common;
using Microsoft.Extensions.DependencyInjection;

namespace BaseTemplate.Infrastructure.Events;

public sealed class DomainEventDispatcher(IServiceProvider serviceProvider) : IDomainEventDispatcher
{
    public async Task DispatchAndClearEvents(IEnumerable<BaseEntity> entitiesWithEvents, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entitiesWithEvents);

        var entities = entitiesWithEvents.ToList();
        var events = entities.SelectMany(e => e.DomainEvents).ToList();

        foreach (var entity in entities)
        {
            entity.ClearDomainEvents();
        }

        foreach (var domainEvent in events)
        {
            var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());

            foreach (var handler in serviceProvider.GetServices(handlerType))
            {
                if (handler is null)
                {
                    continue;
                }

                var handleMethod = handlerType.GetMethod(nameof(IDomainEventHandler<IDomainEvent>.Handle))!;
                await (Task)handleMethod.Invoke(handler, [domainEvent, cancellationToken])!;
            }
        }
    }
}
