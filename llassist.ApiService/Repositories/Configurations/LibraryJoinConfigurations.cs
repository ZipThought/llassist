using llassist.ApiService.Repositories.Converters;
using llassist.Common.Models.Library;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace llassist.ApiService.Repositories.Configurations;

public class CategoryEntryConfiguration : IEntityTypeConfiguration<CategoryEntry>
{
    public void Configure(EntityTypeBuilder<CategoryEntry> builder)
    {
        builder.ToTable("CategoryEntries");
        
        builder.HasKey(ce => new { ce.CategoryId, ce.EntryId });
        
        builder.Property(ce => ce.CategoryId)
            .HasConversion(new UlidToStringConverter());
        
        builder.Property(ce => ce.EntryId)
            .HasConversion(new UlidToStringConverter());

        builder.Property(ce => ce.CreatedAt)
            .HasConversion(new UtcDateTimeOffsetConverter());

        builder.HasOne(ce => ce.Category)
            .WithMany(c => c.Entries)
            .HasForeignKey(ce => ce.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ce => ce.Entry)
            .WithMany(e => e.Categories)
            .HasForeignKey(ce => ce.EntryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class EntryLabelConfiguration : IEntityTypeConfiguration<EntryLabel>
{
    public void Configure(EntityTypeBuilder<EntryLabel> builder)
    {
        builder.ToTable("EntryLabels");
        
        builder.HasKey(el => new { el.EntryId, el.LabelId });
        
        builder.Property(el => el.EntryId)
            .HasConversion(new UlidToStringConverter());
        
        builder.Property(el => el.LabelId)
            .HasConversion(new UlidToStringConverter());

        builder.Property(el => el.CreatedAt)
            .HasConversion(new UtcDateTimeOffsetConverter());

        builder.HasOne(el => el.Entry)
            .WithMany(e => e.Labels)
            .HasForeignKey(el => el.EntryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(el => el.Label)
            .WithMany()
            .HasForeignKey(el => el.LabelId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class ArticleReferenceConfiguration : IEntityTypeConfiguration<ArticleReference>
{
    public void Configure(EntityTypeBuilder<ArticleReference> builder)
    {
        builder.ToTable("ArticleReferences");
        
        builder.HasKey(ar => new { ar.ArticleId, ar.EntryId });
        
        builder.Property(ar => ar.ArticleId)
            .HasConversion(new UlidToStringConverter());
        
        builder.Property(ar => ar.EntryId)
            .HasConversion(new UlidToStringConverter());

        builder.Property(ar => ar.CreatedAt)
            .HasConversion(new UtcDateTimeOffsetConverter());

        builder.HasOne(ar => ar.Article)
            .WithMany()
            .HasForeignKey(ar => ar.ArticleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ar => ar.Entry)
            .WithMany(e => e.ArticleReferences)
            .HasForeignKey(ar => ar.EntryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
} 