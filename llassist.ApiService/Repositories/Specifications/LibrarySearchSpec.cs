namespace llassist.ApiService.Repositories.Specifications;

public class LibrarySearchSpec : BaseSearchSpec
{
    public Ulid? CatalogId { get; set; }
    public string? SearchTerm { get; set; }
    public string? EntryType { get; set; }
    public string? SchemaType { get; set; }
    public DateTimeOffset? FromDate { get; set; }
    public DateTimeOffset? ToDate { get; set; }
    public IList<Ulid> LabelIds { get; set; } = new List<Ulid>();
    public IList<Ulid> CategoryIds { get; set; } = new List<Ulid>();
} 