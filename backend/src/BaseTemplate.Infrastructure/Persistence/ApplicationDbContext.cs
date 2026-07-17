using BaseTemplate.Domain.Entities;
using BaseTemplate.Infrastructure.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Infrastructure.Persistence;

public class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options,
    AuditableEntitySaveChangesInterceptor auditInterceptor)
    : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();

    public DbSet<BrandingSettings> BrandingSettings => Set<BrandingSettings>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(auditInterceptor);
        base.OnConfiguring(optionsBuilder);
    }
}
