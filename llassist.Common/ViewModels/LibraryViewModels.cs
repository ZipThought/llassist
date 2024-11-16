namespace llassist.Common.ViewModels;

public class CatalogViewModel
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Owner { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public IList<EntryViewModel> Entries { get; set; } = [];
}

public class EntryViewModel
{
    public string Id { get; set; } = string.Empty;
    public string EntryType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Citation { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string Identifier { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? PublishedAt { get; set; }
    public IList<ResourceViewModel> Resources { get; set; } = [];
    public IList<string> Labels { get; set; } = [];
    public IDictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
}

public class ResourceViewModel
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long Size { get; set; }
}

public class LabelViewModel
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
}

public class CategoryViewModel
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public int Depth { get; set; }
    public string SchemaType { get; set; } = string.Empty;
    public string? ParentId { get; set; }
    public IList<CategoryViewModel> Children { get; set; } = [];
    public IList<string> EntryIds { get; set; } = [];
}

public class CategoryTreeViewModel
{
    public string SchemaType { get; set; } = string.Empty;
    public IList<CategoryViewModel> RootCategories { get; set; } = [];
}

public class CategoryPathViewModel
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public IList<CategoryBreadcrumbViewModel> Breadcrumbs { get; set; } = [];
}

public class CategoryBreadcrumbViewModel
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Depth { get; set; }
}

public class EntryWithCategoriesViewModel : EntryViewModel
{
    public IDictionary<string, IList<CategoryPathViewModel>> Categories { get; set; }
        = new Dictionary<string, IList<CategoryPathViewModel>>();
}

public class CatalogWithCategoriesViewModel : CatalogViewModel
{
    public IDictionary<string, CategoryTreeViewModel> CategoryTrees { get; set; } 
        = new Dictionary<string, CategoryTreeViewModel>();
}

public class SearchResultViewModel
{
    public EntryViewModel Entry { get; set; } = new EntryViewModel();
    public IList<CategoryPathViewModel> CategoryPaths { get; set; } = [];
} 