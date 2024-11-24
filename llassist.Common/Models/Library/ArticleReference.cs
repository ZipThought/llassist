 using System.Text.Json.Serialization;

namespace llassist.Common.Models.Library;

// Links Articles to Entries
public class ArticleReference
{
    public Ulid ArticleId { get; set; }
    public Ulid EntryId { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    
    [JsonIgnore]
    public virtual Article Article { get; set; } = null!;
    [JsonIgnore]
    public virtual Entry Entry { get; set; } = null!;
}