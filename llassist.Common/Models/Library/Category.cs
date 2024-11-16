using System.Text.Json.Serialization;

namespace llassist.Common.Models.Library;

// Represents a node in hierarchical organization
public class Category : IEntity<Ulid>
{
    public Ulid Id { get; set; } = Ulid.NewUlid();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;  // Full path like "/science/physics/quantum"
    public int Depth { get; set; }                    // Depth in hierarchy
    
    public Ulid CatalogId { get; set; }
    public Ulid? ParentId { get; set; }              // Null for root categories
    
    [JsonIgnore]
    public virtual Catalog Catalog { get; set; } = null!;
    [JsonIgnore]
    public virtual Category? Parent { get; set; }
    
    public ICollection<Category> Children { get; set; } = [];
    public ICollection<CategoryEntry> Entries { get; set; } = [];
    
    public string SchemaType { get; set; } = string.Empty;  // Identifies which classification schema this belongs to
} 