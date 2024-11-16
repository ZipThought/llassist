using System.Text.Json.Serialization;

namespace llassist.Common.Models.Library;

// Represents a catalog entry
public class Entry : IEntity<Ulid>
{
    public Ulid Id { get; set; } = Ulid.NewUlid();
    public string EntryType { get; set; } = string.Empty;  // academic, web, media, correspondence
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Citation { get; set; } = string.Empty;   // Formatted citation text
    public string Source { get; set; } = string.Empty;     // Origin/publisher/platform
    public string Identifier { get; set; } = string.Empty; // DOI/URL/ISBN/etc
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedAt { get; set; }
    public DateTimeOffset? PublishedAt { get; set; }      // Original publication date
    
    public Ulid CatalogId { get; set; }
    [JsonIgnore]
    public virtual Catalog Catalog { get; set; } = null!;
    
    // Flexible metadata storage
    public string Metadata { get; set; } = "{}";
    
    public ICollection<Resource> Resources { get; set; } = [];
    public ICollection<EntryLabel> Labels { get; set; } = [];
    public ICollection<ArticleReference> ArticleReferences { get; set; } = [];
    public ICollection<CategoryEntry> Categories { get; set; } = [];
} 