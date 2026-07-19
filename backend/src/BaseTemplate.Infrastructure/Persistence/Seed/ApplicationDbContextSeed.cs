using BaseTemplate.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BaseTemplate.Infrastructure.Persistence.Seed;

/// <summary>
/// Applies pending migrations and inserts baseline data. Call
/// <see cref="SeedAsync"/> once at startup (see Program.cs) — safe to run
/// repeatedly since it checks for existing data first.
/// </summary>
public static class ApplicationDbContextSeed
{
    public static async Task SeedAsync(ApplicationDbContext context, IServiceProvider serviceProvider, ILogger logger)
    {
        try
        {
            await context.Database.MigrateAsync();

            // Seed the single settings row only if it doesn't exist yet. Once it
            // exists, it's owned by whoever edits it via the settings page —
            // never overwrite an admin's customization back to these defaults
            // on a later startup.
            if (!await context.BrandingSettings.AnyAsync())
            {
                var branding = BrandingSettings.CreateDefault(
                    appName: BrandingDefaults.AppName,
                    appNameAr: BrandingDefaults.AppNameAr,
                    logoUrl: BrandingDefaults.LogoUrl,
                    logoData: null,
                    primaryColor: BrandingDefaults.PrimaryColor,
                    secondaryColor: BrandingDefaults.SecondaryColor);

                await context.BrandingSettings.AddAsync(branding);
                await context.SaveChangesAsync();
                branding.ClearDomainEvents();
                logger.LogInformation("Branding settings seeded with defaults");
            }

            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>("Admin"));
                logger.LogInformation("Admin role created");
            }

            if (await userManager.FindByNameAsync("admin") is null)
            {
                var admin = new AppUser
                {
                    UserName = "admin",
                    Email = "admin@example.com",
                    EmailConfirmed = true,
                };

                var result = await userManager.CreateAsync(admin, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                    logger.LogInformation("Admin user created with default password");
                }
                else
                {
                    logger.LogWarning("Failed to create admin user: {Errors}",
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
    }
}
