using BaseTemplate.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaseTemplate.Infrastructure.Persistence.Configurations;

public sealed class BrandingSettingsConfiguration : IEntityTypeConfiguration<BrandingSettings>
{
    public void Configure(EntityTypeBuilder<BrandingSettings> builder)
    {
        builder.ToTable("BrandingSettings");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.AppName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(b => b.AppNameAr)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(b => b.LogoUrl)
            .HasMaxLength(2048);

        builder.Property(b => b.LogoData);

        builder.Property(b => b.PrimaryColor)
            .IsRequired()
            .HasMaxLength(7);

        builder.Property(b => b.SecondaryColor)
            .IsRequired()
            .HasMaxLength(7);

        builder.Property(b => b.CreatedAt).IsRequired();
        builder.Property(b => b.UpdatedAt);
        builder.Property(b => b.IsDeleted).IsRequired().HasDefaultValue(false);

        builder.HasQueryFilter(b => !b.IsDeleted);

        builder.Ignore(b => b.DomainEvents);
    }
}
