using System.Text.Json.Serialization;

namespace llassist.Common.Models.Library;

// Organization within a catalog
public class Collection : IEntity<Ulid>
{
    public Ulid Id { get; set; } = Ulid.NewUlid();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedAt { get; set; }
    
    public Ulid CatalogId { get; set; }
    [JsonIgnore]
    public virtual Catalog Catalog { get; set; } = null!;
    
    public ICollection<Entry> Entries { get; set; } = [];
} 