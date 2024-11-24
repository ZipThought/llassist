using llassist.Common.Models;
using llassist.Common.Models.Library;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace llassist.ApiService.Repositories;
public class ApplicationDbContext : DbContext
{
    public DbSet<ArticleKeySemantic> ArticleKeySemantics { get; set; }
    public DbSet<ArticleRelevance> ArticleRelevances { get; set; }
    public DbSet<Article> Articles { get; set; }
    public DbSet<EstimateRelevanceJob> EstimateRelevanceJobs { get; set; }
    public DbSet<ProjectDefinition> ProjectDefinitions { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<QuestionDefinition> QuestionDefinitions { get; set; }
    public DbSet<ResearchQuestion> ResearchQuestions { get; set; }
    public DbSet<Snapshot> Snapshots { get; set; }
    public DbSet<AppSetting> AppSettings { get; set; }
    public DbSet<Catalog> Catalogs { get; set; }
    public DbSet<Entry> Entries { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Label> Labels { get; set; }
    public DbSet<Resource> Resources { get; set; }
    public DbSet<Collection> Collections { get; set; }
    public DbSet<CategoryEntry> CategoryEntries { get; set; }
    public DbSet<EntryLabel> EntryLabels { get; set; }
    public DbSet<ArticleReference> ArticleReferences { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
