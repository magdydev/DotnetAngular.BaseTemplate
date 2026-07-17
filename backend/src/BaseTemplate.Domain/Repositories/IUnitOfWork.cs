namespace BaseTemplate.Domain.Repositories;

/// <summary>
/// Coordinates repositories that must be persisted together as a single transaction.
/// Add a property per aggregate repository as the domain grows.
/// </summary>
public interface IUnitOfWork
{
    IProductRepository Products { get; }

    IBrandingSettingsRepository BrandingSettings { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
