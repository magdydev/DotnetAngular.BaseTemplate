using BaseTemplate.Domain.Entities;

namespace BaseTemplate.Domain.Repositories;

/// <summary>
/// Aggregate-specific repository. Add query methods here as new use cases need them —
/// keep the generic <see cref="IRepository{TEntity}"/> free of entity-specific concerns.
/// </summary>
public interface IProductRepository : IRepository<Product>
{
    Task<Product?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default);
}
