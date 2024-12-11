 using System.Text.Json.Serialization;

namespace llassist.Common.Models.Library;

// Junction table for Entry-Label relationship
public class EntryLabel
{
    public Ulid EntryId { get; set; }
    public Ulid LabelId { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    
    [JsonIgnore]
    public virtual Entry Entry { get; set; } = null!;
    [JsonIgnore]
    public virtual Label Label { get; set; } = null!;
}