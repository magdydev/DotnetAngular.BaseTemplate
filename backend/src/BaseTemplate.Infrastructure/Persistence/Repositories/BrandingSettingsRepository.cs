using BaseTemplate.Domain.Entities;
using BaseTemplate.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Infrastructure.Persistence.Repositories;

public sealed class BrandingSettingsRepository(ApplicationDbContext context)
    : RepositoryBase<BrandingSettings>(context), IBrandingSettingsRepository
{
    public Task<BrandingSettings?> GetCurrentAsync(CancellationToken cancellationToken = default) =>
        DbSet.OrderBy(b => b.CreatedAt).FirstOrDefaultAsync(cancellationToken);
}
