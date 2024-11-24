using System.Text.Json.Serialization;

namespace llassist.Common.Models.Library;

// Root level container for a Library
public class Catalog : IEntity<Ulid>
{
    public Ulid Id { get; set; } = Ulid.NewUlid();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Owner { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedAt { get; set; }
    
    public ICollection<Entry> Entries { get; set; } = [];
    public ICollection<Label> Labels { get; set; } = [];
    public ICollection<Category> Categories { get; set; } = [];
} 