using llassist.ApiService.Repositories.Converters;
using llassist.Common.Models.Library;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace llassist.ApiService.Repositories.Configurations;

public class EntryConfiguration : IEntityTypeConfiguration<Entry>
{
    public void Configure(EntityTypeBuilder<Entry> builder)
    {
        builder.ToTable("Entries");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasConversion(new UlidToStringConverter());

        builder.Property(e => e.EntryType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.Description)
            .HasMaxLength(5000);

        builder.Property(e => e.CitationFields)
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v.Fields, JsonSerializerOptions.Default),
                json => new DataFieldCollection(JsonSerializer.Deserialize<List<DataField>>(json, JsonSerializerOptions.Default))
            );

        builder.Property(e => e.MetadataFields)
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v.Fields, JsonSerializerOptions.Default),
                json => new DataFieldCollection(JsonSerializer.Deserialize<List<DataField>>(json, JsonSerializerOptions.Default))
            );

        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasConversion(new UtcDateTimeOffsetConverter());

        builder.Property(e => e.UpdatedAt)
            .HasConversion(new UtcDateTimeOffsetConverter());

        builder.Property(e => e.PublishedAt)
            .HasConversion(new UtcDateTimeOffsetConverter());

        builder.Property(e => e.CatalogId)
            .HasConversion(new UlidToStringConverter());

        builder.HasMany(e => e.Resources)
            .WithOne(r => r.Entry)
            .HasForeignKey(r => r.EntryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Labels)
            .WithOne(el => el.Entry)
            .HasForeignKey(el => el.EntryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Categories)
            .WithOne(ce => ce.Entry)
            .HasForeignKey(ce => ce.EntryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
} 