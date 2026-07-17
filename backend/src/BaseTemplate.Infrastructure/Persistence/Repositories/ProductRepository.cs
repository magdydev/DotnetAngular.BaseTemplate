using BaseTemplate.Domain.Entities;
using BaseTemplate.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Infrastructure.Persistence.Repositories;

public sealed class ProductRepository(ApplicationDbContext context)
    : RepositoryBase<Product>(context), IProductRepository
{
    public Task<Product?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default) =>
        DbSet.FirstOrDefaultAsync(p => p.Sku == sku.ToUpperInvariant(), cancellationToken);
}
