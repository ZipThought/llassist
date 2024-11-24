using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace llassist.Common.Models.Library;

// Represents a node in hierarchical organization
public class Category : IEntity<Ulid>
{
    public Ulid Id { get; set; } = Ulid.NewUlid();
    
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(2000)]
    public string Description { get; set; } = string.Empty;
    
    // Full path like "/science/physics/quantum"
    [Required]
    [StringLength(1000)]
    public string Path { get; set; } = string.Empty;
    
    // Depth in hierarchy, must correspond to the number of slashes in the path (root is 0)
    public int Depth { get; set; }
    
    public Ulid CatalogId { get; set; }
    public Ulid? ParentId { get; set; }
    
    // Identifies which classification schema this belongs to
    // Examples: "subject", "year", "topic", "grade"
    [Required]
    [StringLength(100)]
    public string SchemaType { get; set; } = string.Empty;
    
    [JsonIgnore]
    public virtual Catalog Catalog { get; set; } = null!;
    [JsonIgnore]
    public virtual Category? Parent { get; set; }
    
    public ICollection<Category> Children { get; set; } = [];
    public ICollection<CategoryEntry> Entries { get; set; } = [];
} 