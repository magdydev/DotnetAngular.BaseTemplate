using BaseTemplate.Domain.Repositories;
using BaseTemplate.Infrastructure.Persistence.Repositories;

namespace BaseTemplate.Infrastructure.Persistence;

public sealed class UnitOfWork(ApplicationDbContext context) : IUnitOfWork
{
    private IProductRepository? _products;

    public IProductRepository Products => _products ??= new ProductRepository(context);

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}
