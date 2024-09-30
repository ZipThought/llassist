using llassist.Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace llassist.ApiService.Repositories.Configurations;

public class EstimateRelevanceJobConfiguration : IEntityTypeConfiguration<EstimateRelevanceJob>
{
    public void Configure(EntityTypeBuilder<EstimateRelevanceJob> builder)
    {
        builder.ToTable("EstimateRelevanceJobs");
        builder.HasKey(j => j.Id);
        builder.HasIndex(j => j.ProjectId);
        builder.Property(j => j.CreatedAt)
            .HasConversion(new UtcDateTimeOffsetConverter());
        builder.OwnsOne(j => j.ResearchQuestions, builder =>
        {
            builder.ToJson();
            builder.OwnsMany(j => j.Questions);
        });
    }
}

public class UtcDateTimeOffsetConverter : ValueConverter<DateTimeOffset, DateTime>
{
    // Always stores in UTC
    public UtcDateTimeOffsetConverter()
        : base(
            dto => dto.UtcDateTime,
            dt => new DateTimeOffset(DateTime.SpecifyKind(dt, DateTimeKind.Utc)))
    {
    }
}
