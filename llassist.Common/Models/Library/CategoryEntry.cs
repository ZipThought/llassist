using System.Text.Json.Serialization;

namespace llassist.Common.Models.Library;

// Junction table for Category-Entry relationship
public class CategoryEntry
{
    public Ulid CategoryId { get; set; }
    public Ulid EntryId { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    
    [JsonIgnore]
    public virtual Category Category { get; set; } = null!;
    [JsonIgnore]
    public virtual Entry Entry { get; set; } = null!;
} 