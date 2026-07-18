using BaseTemplate.Domain.Repositories;

namespace BaseTemplate.Infrastructure.Persistence;

public sealed class UnitOfWork(ApplicationDbContext context, IBrandingSettingsRepository brandingSettings)
    : IUnitOfWork
{
    public IBrandingSettingsRepository BrandingSettings => brandingSettings;

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}
