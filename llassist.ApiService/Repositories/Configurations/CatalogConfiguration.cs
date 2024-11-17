using llassist.ApiService.Repositories.Converters;
using llassist.Common.Models.Library;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace llassist.ApiService.Repositories.Configurations;

public class CatalogConfiguration : IEntityTypeConfiguration<Catalog>
{
    public void Configure(EntityTypeBuilder<Catalog> builder)
    {
        builder.ToTable("Catalogs");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
            .HasConversion(new UlidToStringConverter());

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Description)
            .HasMaxLength(2000);

        builder.Property(c => c.Owner)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.CreatedAt)
            .IsRequired()
            .HasConversion(new UtcDateTimeOffsetConverter());

        builder.Property(c => c.UpdatedAt)
            .HasConversion(new UtcDateTimeOffsetConverter());

        builder.HasMany(c => c.Entries)
            .WithOne(e => e.Catalog)
            .HasForeignKey(e => e.CatalogId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Labels)
            .WithOne(l => l.Catalog)
            .HasForeignKey(l => l.CatalogId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Categories)
            .WithOne(c => c.Catalog)
            .HasForeignKey(c => c.CatalogId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}