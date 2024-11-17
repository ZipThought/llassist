 using llassist.ApiService.Repositories.Converters;
using llassist.Common.Models.Library;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
            .HasConversion(new UlidToStringConverter());

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Description)
            .HasMaxLength(2000);

        builder.Property(c => c.Path)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(c => c.Depth)
            .IsRequired();

        builder.Property(c => c.SchemaType)
            .HasMaxLength(100);

        builder.Property(c => c.CatalogId)
            .HasConversion(new UlidToStringConverter());

        builder.Property(c => c.ParentId)
            .HasConversion(new UlidToStringConverter());

        builder.HasOne(c => c.Parent)
            .WithMany(c => c.Children)
            .HasForeignKey(c => c.ParentId)
            .IsRequired(false)  // Allow null for root categories
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Catalog)
            .WithMany(c => c.Categories)
            .HasForeignKey(c => c.CatalogId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Entries)
            .WithOne(ce => ce.Category)
            .HasForeignKey(ce => ce.CategoryId);
    }
}