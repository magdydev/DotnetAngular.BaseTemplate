using BaseTemplate.Domain.Entities;

namespace BaseTemplate.Domain.Repositories;

public interface IBrandingSettingsRepository : IRepository<BrandingSettings>
{
    /// <summary>Returns the single settings row, or null if it hasn't been seeded yet.</summary>
    Task<BrandingSettings?> GetCurrentAsync(CancellationToken cancellationToken = default);
}
