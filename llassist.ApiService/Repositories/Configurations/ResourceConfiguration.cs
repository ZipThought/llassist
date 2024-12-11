using llassist.ApiService.Repositories.Converters;
using llassist.Common.Models.Library;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace llassist.ApiService.Repositories.Configurations;

public class ResourceConfiguration : IEntityTypeConfiguration<Resource>
{
    public void Configure(EntityTypeBuilder<Resource> builder)
    {
        builder.ToTable("Resources");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id)
            .HasConversion(new UlidToStringConverter());

        builder.Property(r => r.Type)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(r => r.Path)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(r => r.ContentType)
            .HasMaxLength(100);

        builder.Property(r => r.Hash)
            .HasMaxLength(128);

        builder.Property(r => r.Size)
            .HasColumnType("bigint");

        builder.Property(r => r.CreatedAt)
            .IsRequired()
            .HasConversion(new UtcDateTimeOffsetConverter());

        builder.Property(r => r.EntryId)
            .HasConversion(new UlidToStringConverter());

        builder.HasOne(r => r.Entry)
            .WithMany(e => e.Resources)
            .HasForeignKey(r => r.EntryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
} 