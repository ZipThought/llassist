using llassist.ApiService.Repositories.Converters;
using llassist.Common.Models.Library;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace llassist.ApiService.Repositories.Configurations;

public class LabelConfiguration : IEntityTypeConfiguration<Label>
{
    public void Configure(EntityTypeBuilder<Label> builder)
    {
        builder.ToTable("Labels");

        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id)
            .HasConversion(new UlidToStringConverter());

        builder.Property(l => l.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(l => l.Description)
            .HasMaxLength(500);

        builder.Property(l => l.Color)
            .HasMaxLength(7);  // #RRGGBB format

        builder.Property(l => l.CatalogId)
            .HasConversion(new UlidToStringConverter());

        builder.HasOne(l => l.Catalog)
            .WithMany(c => c.Labels)
            .HasForeignKey(l => l.CatalogId)
            .OnDelete(DeleteBehavior.Cascade);
    }
} 