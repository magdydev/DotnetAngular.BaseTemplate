using BaseTemplate.Domain.Common;

namespace BaseTemplate.Domain.Repositories;

/// <summary>
/// Generic repository contract. Lives in the Domain layer so use cases can depend
/// on it without knowing about EF Core or any other persistence technology.
/// Concrete entity repositories (e.g. <see cref="IProductRepository"/>) extend this
/// with query methods specific to that aggregate.
/// </summary>
public interface IRepository<TEntity> where TEntity : BaseEntity
{
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TEntity>> ListAllAsync(CancellationToken cancellationToken = default);

    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    void Update(TEntity entity);

    void Remove(TEntity entity);
}
