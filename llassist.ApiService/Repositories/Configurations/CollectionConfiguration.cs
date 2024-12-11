using llassist.ApiService.Repositories.Converters;
using llassist.Common.Models.Library;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace llassist.ApiService.Repositories.Configurations;

public class CollectionConfiguration : IEntityTypeConfiguration<Collection>
{
    public void Configure(EntityTypeBuilder<Collection> builder)
    {
        builder.ToTable("Collections");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
            .HasConversion(new UlidToStringConverter());

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Description)
            .HasMaxLength(2000);

        builder.Property(c => c.CreatedAt)
            .HasConversion(new UtcDateTimeOffsetConverter());

        builder.Property(c => c.UpdatedAt)
            .HasConversion(new UtcDateTimeOffsetConverter());

        builder.Property(c => c.CatalogId)
            .HasConversion(new UlidToStringConverter());

        builder.HasOne(c => c.Catalog)
            .WithMany()
            .HasForeignKey(c => c.CatalogId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Entries)
            .WithMany()
            .UsingEntity("CollectionEntries");
    }
} 