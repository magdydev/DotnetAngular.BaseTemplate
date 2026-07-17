using BaseTemplate.Domain.Common;

namespace BaseTemplate.Application.Common.Interfaces;

/// <summary>
/// Publishes the domain events buffered on tracked entities and clears them.
/// Implemented in Infrastructure, invoked by the UnitOfWork right after a
/// successful SaveChanges so handlers only ever see committed state.
/// </summary>
public interface IDomainEventDispatcher
{
    Task DispatchAndClearEvents(IEnumerable<BaseEntity> entitiesWithEvents, CancellationToken cancellationToken = default);
}
