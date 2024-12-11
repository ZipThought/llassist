 using System.Text.Json.Serialization;

namespace llassist.Common.Models.Library;

// Label for categorization within a Catalog
public class Label : IEntity<Ulid>
{
    public Ulid Id { get; set; } = Ulid.NewUlid();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public Ulid CatalogId { get; set; }
    
    [JsonIgnore]
    public virtual Catalog Catalog { get; set; } = null!;
}