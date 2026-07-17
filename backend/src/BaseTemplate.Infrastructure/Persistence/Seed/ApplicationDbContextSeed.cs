using BaseTemplate.Domain.Entities;
using BaseTemplate.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BaseTemplate.Infrastructure.Persistence.Seed;

/// <summary>
/// Applies pending migrations and inserts baseline data. Call
/// <see cref="SeedAsync"/> once at startup (see Program.cs) — safe to run
/// repeatedly since it checks for existing data first.
/// </summary>
public static class ApplicationDbContextSeed
{
    public static async Task SeedAsync(ApplicationDbContext context, ILogger logger)
    {
        try
        {
            await context.Database.MigrateAsync();

            if (!await context.Products.AnyAsync())
            {
                var product = Product.Create(
                    name: "Sample Product",
                    sku: "SAMPLE-001",
                    price: Money.Create(9.99m, "USD"),
                    description: "Seed data demonstrating the Product aggregate. Safe to delete.");

                await context.Products.AddAsync(product);
                await context.SaveChangesAsync();
                product.ClearDomainEvents();
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
    }
}
