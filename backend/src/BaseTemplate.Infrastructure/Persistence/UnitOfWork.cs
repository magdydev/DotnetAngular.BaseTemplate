using BaseTemplate.Domain.Repositories;
using BaseTemplate.Infrastructure.Persistence.Repositories;

namespace BaseTemplate.Infrastructure.Persistence;

public sealed class UnitOfWork(ApplicationDbContext context) : IUnitOfWork
{
    private IProductRepository? _products;
    private IBrandingSettingsRepository? _brandingSettings;

    public IProductRepository Products => _products ??= new ProductRepository(context);

    public IBrandingSettingsRepository BrandingSettings => _brandingSettings ??= new BrandingSettingsRepository(context);

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}
