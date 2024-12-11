using System.Text.Json.Serialization;

namespace llassist.Common.Models.Library;

// Represents any attached resource
public class Resource : IEntity<Ulid>
{
    public Ulid Id { get; set; } = Ulid.NewUlid();
    public Ulid EntryId { get; set; }
    
    [JsonIgnore]
    public virtual Entry Entry { get; set; } = null!;
    
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;      // file, url, embedded
    public string ContentType { get; set; } = string.Empty;
    public long Size { get; set; }
    public string Hash { get; set; } = string.Empty;      // For integrity check
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
} 