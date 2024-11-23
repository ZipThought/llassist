using llassist.Common;
using llassist.Common.Models.Library;
using llassist.Common.ViewModels;
using llassist.ApiService.Repositories.Specifications;
using System.Text.Json;

namespace llassist.ApiService.Services;

public interface ILibraryService
{
    // Catalog operations
    Task<CatalogViewModel> CreateCatalogAsync(string name, string description, string owner);
    Task<CatalogViewModel?> GetCatalogAsync(Ulid id);
    Task<IEnumerable<CatalogViewModel>> GetAllCatalogsAsync();
    Task<CatalogViewModel?> UpdateCatalogAsync(Ulid id, string name, string description);
    Task<bool> DeleteCatalogAsync(Ulid id);

    // Entry operations
    Task<EntryViewModel> CreateEntryAsync(Ulid catalogId, EntryViewModel entry);
    Task<EntryViewModel?> GetEntryAsync(Ulid id);
    Task<IEnumerable<EntryViewModel>> GetEntriesByCatalogAsync(Ulid catalogId);
    Task<EntryViewModel?> UpdateEntryAsync(Ulid id, CreateEditEntryViewModel entryVm);
    Task<bool> DeleteEntryAsync(Ulid id);

    // Category operations
    Task<CategoryViewModel> CreateCategoryAsync(Ulid catalogId, CategoryViewModel category);
    Task<CategoryTreeViewModel> GetCategoryTreeAsync(Ulid catalogId, string schemaType);
    Task<bool> AssignEntryToCategoryAsync(Ulid entryId, Ulid categoryId);
    Task<bool> RemoveEntryFromCategoryAsync(Ulid entryId, Ulid categoryId);

    // Label operations
    Task<LabelViewModel> CreateLabelAsync(Ulid catalogId, LabelViewModel label);
    Task<bool> AssignLabelToEntryAsync(Ulid entryId, Ulid labelId);
    Task<bool> RemoveLabelFromEntryAsync(Ulid entryId, Ulid labelId);

    // Category operations
    Task<CategoryViewModel?> GetCategoryAsync(Ulid id);
    Task<IEnumerable<EntryViewModel>> GetEntriesByCategoryAsync(Ulid categoryId);

    Task<bool> DeleteCategoryAsync(Ulid id);
    Task<CategoryViewModel?> UpdateCategoryAsync(Ulid id, CategoryViewModel categoryVm);
}

public class LibraryService : ILibraryService
{
    private readonly ICRUDRepository<Ulid, Catalog, BaseSearchSpec> _catalogRepository;
    private readonly ICRUDRepository<Ulid, Entry, BaseSearchSpec> _entryRepository;
    private readonly ICRUDRepository<Ulid, Category, BaseSearchSpec> _categoryRepository;
    private readonly ICRUDRepository<Ulid, Label, BaseSearchSpec> _labelRepository;
    private readonly ILogger<LibraryService> _logger;

    public LibraryService(
        ICRUDRepository<Ulid, Catalog, BaseSearchSpec> catalogRepository,
        ICRUDRepository<Ulid, Entry, BaseSearchSpec> entryRepository,
        ICRUDRepository<Ulid, Category, BaseSearchSpec> categoryRepository,
        ICRUDRepository<Ulid, Label, BaseSearchSpec> labelRepository,
        ILogger<LibraryService> logger)
    {
        _catalogRepository = catalogRepository;
        _entryRepository = entryRepository;
        _categoryRepository = categoryRepository;
        _labelRepository = labelRepository;
        _logger = logger;
    }

    public async Task<CatalogViewModel> CreateCatalogAsync(string name, string description, string owner)
    {
        var catalog = new Catalog
        {
            Name = name,
            Description = description,
            Owner = owner
        };

        var created = await _catalogRepository.CreateAsync(catalog);
        return MapToCatalogViewModel(created);
    }

    public async Task<CatalogViewModel?> GetCatalogAsync(Ulid id)
    {
        var catalog = await _catalogRepository.ReadAsync(id);
        return catalog != null ? MapToCatalogViewModel(catalog) : null;
    }

    public async Task<IEnumerable<CatalogViewModel>> GetAllCatalogsAsync()
    {
        var catalogs = await _catalogRepository.ReadAllAsync();
        return catalogs.Select(MapToCatalogViewModel);
    }

    public async Task<CatalogViewModel?> UpdateCatalogAsync(Ulid id, string name, string description)
    {
        var catalog = await _catalogRepository.ReadAsync(id);
        if (catalog == null) return null;

        catalog.Name = name;
        catalog.Description = description;
        catalog.UpdatedAt = DateTimeOffset.UtcNow;

        var updated = await _catalogRepository.UpdateAsync(catalog);
        return MapToCatalogViewModel(updated);
    }

    public async Task<bool> DeleteCatalogAsync(Ulid id)
    {
        return await _catalogRepository.DeleteAsync(id);
    }

    public async Task<EntryViewModel> CreateEntryAsync(Ulid catalogId, EntryViewModel entryVm)
    {
        var entry = new Entry
        {
            CatalogId = catalogId,
            EntryType = entryVm.EntryType,
            Title = entryVm.Title,
            Description = entryVm.Description,
            Citation = entryVm.Citation,
            Source = entryVm.Source,
            Identifier = entryVm.Identifier,
            PublishedAt = entryVm.PublishedAt,
            Metadata = JsonSerializer.Serialize(entryVm.Metadata)
        };

        var created = await _entryRepository.CreateAsync(entry);
        return MapToEntryViewModel(created);
    }

    public async Task<EntryViewModel?> GetEntryAsync(Ulid id)
    {
        var entry = await _entryRepository.ReadAsync(id);
        return entry != null ? MapToEntryViewModel(entry) : null;
    }

    public async Task<IEnumerable<EntryViewModel>> GetEntriesByCatalogAsync(Ulid catalogId)
    {
        var catalog = await _catalogRepository.ReadAsync(catalogId);
        return catalog?.Entries.Select(MapToEntryViewModel) ?? Enumerable.Empty<EntryViewModel>();
    }

    public async Task<EntryViewModel?> UpdateEntryAsync(Ulid id, CreateEditEntryViewModel entryVm)
    {
        var entry = await _entryRepository.ReadAsync(id);
        if (entry == null) return null;

        entry.Title = entryVm.Title;
        entry.Description = entryVm.Description;
        entry.EntryType = entryVm.EntryType;
        entry.Source = entryVm.Source;
        entry.Citation = entryVm.Citation;
        entry.Identifier = entryVm.Identifier;
        entry.PublishedAt = entryVm.PublishedAt;
        entry.UpdatedAt = DateTimeOffset.UtcNow;

        var updated = await _entryRepository.UpdateAsync(entry);
        return MapToEntryViewModel(updated);
    }

    public async Task<bool> DeleteEntryAsync(Ulid id)
    {
        return await _entryRepository.DeleteAsync(id);
    }

    public async Task<CategoryViewModel> CreateCategoryAsync(Ulid catalogId, CategoryViewModel categoryVm)
    {
        if (string.IsNullOrWhiteSpace(categoryVm.Name))
        {
            throw new ArgumentException("Category name is required");
        }

        if (string.IsNullOrWhiteSpace(categoryVm.SchemaType))
        {
            throw new ArgumentException("Schema type is required");
        }

        var parent = categoryVm.ParentId != null ? 
            await _categoryRepository.ReadAsync(Ulid.Parse(categoryVm.ParentId)) : null;

        var category = new Category
        {
            CatalogId = catalogId,
            Name = categoryVm.Name.Trim(),
            Description = categoryVm.Description?.Trim() ?? string.Empty,
            SchemaType = categoryVm.SchemaType,
            ParentId = parent?.Id,
            Depth = (parent?.Depth ?? -1) + 1
        };

        // Calculate the full path
        category.Path = parent != null ? 
            $"{parent.Path}/{category.Name}" : $"/{category.Name}";

        var created = await _categoryRepository.CreateAsync(category);
        return MapToCategoryViewModel(created);
    }

    public async Task<CategoryTreeViewModel> GetCategoryTreeAsync(Ulid catalogId, string schemaType)
    {
        _logger.LogInformation("Getting category tree for catalog {CatalogId} and schema {SchemaType}", 
            catalogId, schemaType);

        var catalog = await _catalogRepository.ReadAsync(catalogId);
        if (catalog == null)
        {
            _logger.LogWarning("Catalog {CatalogId} not found", catalogId);
            return new CategoryTreeViewModel 
            { 
                SchemaType = schemaType,
                RootCategories = new List<CategoryViewModel>()
            };
        }

        _logger.LogInformation("Found {Count} categories for catalog", catalog.Categories.Count);

        // Get categories for this schema type
        var schemaCategories = catalog.Categories
            .Where(c => c.SchemaType == schemaType)
            .ToList();

        _logger.LogInformation("Found {Count} categories for schema type {SchemaType}", 
            schemaCategories.Count, schemaType);

        // Build tree structure
        var rootCategories = schemaCategories
            .Where(c => c.ParentId == null)
            .OrderBy(c => c.Name)
            .Select(c => MapToCategoryViewModel(c, schemaCategories))
            .ToList();

        return new CategoryTreeViewModel
        {
            SchemaType = schemaType,
            RootCategories = rootCategories
        };
    }

    private CategoryViewModel MapToCategoryViewModel(Category category, List<Category> allCategories)
    {
        var children = allCategories
            .Where(c => c.ParentId == category.Id)
            .OrderBy(c => c.Name)
            .Select(c => MapToCategoryViewModel(c, allCategories))
            .ToList();

        return new CategoryViewModel
        {
            Id = category.Id.ToString(),
            Name = category.Name,
            Description = category.Description,
            Path = category.Path,
            Depth = category.Depth,
            SchemaType = category.SchemaType,
            ParentId = category.ParentId?.ToString(),
            Children = children,
            EntryIds = category.Entries.Select(ce => ce.EntryId.ToString()).ToList()
        };
    }

    public async Task<bool> AssignEntryToCategoryAsync(Ulid entryId, Ulid categoryId)
    {
        var entry = await _entryRepository.ReadAsync(entryId);
        var category = await _categoryRepository.ReadAsync(categoryId);

        if (entry == null || category == null) return false;

        var categoryEntry = new CategoryEntry
        {
            EntryId = entryId,
            CategoryId = categoryId
        };

        entry.Categories.Add(categoryEntry);
        await _entryRepository.UpdateAsync(entry);
        return true;
    }

    public async Task<bool> RemoveEntryFromCategoryAsync(Ulid entryId, Ulid categoryId)
    {
        var entry = await _entryRepository.ReadAsync(entryId);
        if (entry == null) return false;

        var categoryEntry = entry.Categories.FirstOrDefault(ce => 
            ce.EntryId == entryId && ce.CategoryId == categoryId);

        if (categoryEntry == null) return false;

        entry.Categories.Remove(categoryEntry);
        await _entryRepository.UpdateAsync(entry);
        return true;
    }

    public async Task<LabelViewModel> CreateLabelAsync(Ulid catalogId, LabelViewModel labelVm)
    {
        var label = new Label
        {
            CatalogId = catalogId,
            Name = labelVm.Name,
            Description = labelVm.Description,
            Color = labelVm.Color
        };

        var created = await _labelRepository.CreateAsync(label);
        return MapToLabelViewModel(created);
    }

    public async Task<bool> AssignLabelToEntryAsync(Ulid entryId, Ulid labelId)
    {
        var entry = await _entryRepository.ReadAsync(entryId);
        var label = await _labelRepository.ReadAsync(labelId);

        if (entry == null || label == null) return false;

        var entryLabel = new EntryLabel
        {
            EntryId = entryId,
            LabelId = labelId
        };

        entry.Labels.Add(entryLabel);
        await _entryRepository.UpdateAsync(entry);
        return true;
    }

    public async Task<bool> RemoveLabelFromEntryAsync(Ulid entryId, Ulid labelId)
    {
        var entry = await _entryRepository.ReadAsync(entryId);
        if (entry == null) return false;

        var entryLabel = entry.Labels.FirstOrDefault(el => 
            el.EntryId == entryId && el.LabelId == labelId);

        if (entryLabel == null) return false;

        entry.Labels.Remove(entryLabel);
        await _entryRepository.UpdateAsync(entry);
        return true;
    }

    public async Task<CategoryViewModel?> GetCategoryAsync(Ulid id)
    {
        var category = await _categoryRepository.ReadAsync(id);
        return category != null ? MapToCategoryViewModel(category) : null;
    }

    public async Task<IEnumerable<EntryViewModel>> GetEntriesByCategoryAsync(Ulid categoryId)
    {
        var category = await _categoryRepository.ReadAsync(categoryId);
        if (category == null) return Enumerable.Empty<EntryViewModel>();
        
        return category.Entries
            .Select(ce => ce.Entry)
            .Select(MapToEntryViewModel);
    }

    public async Task<bool> DeleteCategoryAsync(Ulid id)
    {
        var category = await _categoryRepository.ReadAsync(id);
        if (category == null)
        {
            return false;
        }

        // First delete all child categories
        var children = await GetAllChildCategories(id);
        foreach (var child in children)
        {
            await _categoryRepository.DeleteAsync(child.Id);
        }

        return await _categoryRepository.DeleteAsync(id);
    }

    public async Task<CategoryViewModel?> UpdateCategoryAsync(Ulid id, CategoryViewModel categoryVm)
    {
        var category = await _categoryRepository.ReadAsync(id);
        if (category == null)
        {
            return null;
        }

        // Prevent self-reference
        if (!string.IsNullOrEmpty(categoryVm.ParentId) && categoryVm.ParentId == category.Id.ToString())
        {
            throw new InvalidOperationException("A category cannot be its own parent");
        }

        // Check for circular reference only if ParentId is provided
        if (!string.IsNullOrEmpty(categoryVm.ParentId))
        {
            var parentId = Ulid.Parse(categoryVm.ParentId);
            if (await WouldCreateCircularReference(id, parentId))
            {
                throw new InvalidOperationException("This operation would create a circular reference");
            }
        }

        category.Name = categoryVm.Name.Trim();
        category.Description = categoryVm.Description?.Trim() ?? string.Empty;
        
        // Handle moving to root (null ParentId) or to a new parent
        if (string.IsNullOrEmpty(categoryVm.ParentId))
        {
            // Moving to root
            category.ParentId = null;
            category.Depth = 0;
            category.Path = $"/{category.Name}";
        }
        else if (categoryVm.ParentId != category.ParentId?.ToString())
        {
            // Moving to new parent
            var parent = await _categoryRepository.ReadAsync(Ulid.Parse(categoryVm.ParentId));
            if (parent != null)
            {
                category.ParentId = parent.Id;
                category.Depth = parent.Depth + 1;
                category.Path = $"{parent.Path}/{category.Name}";
            }
        }

        var updated = await _categoryRepository.UpdateAsync(category);
        return MapToCategoryViewModel(updated);
    }

    private async Task<bool> WouldCreateCircularReference(Ulid categoryId, Ulid newParentId)
    {
        var current = await _categoryRepository.ReadAsync(newParentId);
        var visited = new HashSet<Ulid>();

        while (current != null && current.ParentId.HasValue)
        {
            // If we've seen this category before or if it's the category we're trying to move, it's circular
            if (!visited.Add(current.Id) || current.Id == categoryId)
            {
                return true;
            }
            current = await _categoryRepository.ReadAsync(current.ParentId.Value);
        }

        return false;
    }

    private async Task<IEnumerable<Category>> GetAllChildCategories(Ulid parentId)
    {
        var children = new List<Category>();
        var directChildren = (await _categoryRepository.ReadAllAsync())
            .Where(c => c.ParentId == parentId)
            .ToList();

        foreach (var child in directChildren)
        {
            children.Add(child);
            children.AddRange(await GetAllChildCategories(child.Id));
        }

        return children;
    }

    private static CatalogViewModel MapToCatalogViewModel(Catalog catalog)
    {
        return new CatalogViewModel
        {
            Id = catalog.Id.ToString(),
            Name = catalog.Name,
            Description = catalog.Description,
            Owner = catalog.Owner,
            CreatedAt = catalog.CreatedAt,
            Entries = catalog.Entries.Select(MapToEntryViewModel).ToList()
        };
    }

    private static EntryViewModel MapToEntryViewModel(Entry entry)
    {
        return new EntryViewModel
        {
            Id = entry.Id.ToString(),
            EntryType = entry.EntryType,
            Title = entry.Title,
            Description = entry.Description,
            Citation = entry.Citation,
            Source = entry.Source,
            Identifier = entry.Identifier,
            CreatedAt = entry.CreatedAt,
            PublishedAt = entry.PublishedAt,
            Resources = entry.Resources.Select(MapToResourceViewModel).ToList(),
            Labels = entry.Labels.Select(l => l.Label.Name).ToList(),
            Categories = entry.Categories
                .Select(ce => MapToCategoryViewModel(ce.Category))
                .ToList(),
            Metadata = JsonSerializer.Deserialize<Dictionary<string, object>>(entry.Metadata) 
                ?? new Dictionary<string, object>()
        };
    }

    private static ResourceViewModel MapToResourceViewModel(Resource resource)
    {
        return new ResourceViewModel
        {
            Id = resource.Id.ToString(),
            Name = resource.Name,
            Type = resource.Type,
            ContentType = resource.ContentType,
            Size = resource.Size
        };
    }

    private static CategoryViewModel MapToCategoryViewModel(Category category)
    {
        return new CategoryViewModel
        {
            Id = category.Id.ToString(),
            Name = category.Name,
            Description = category.Description,
            Path = category.Path,
            Depth = category.Depth,
            SchemaType = category.SchemaType,
            ParentId = category.ParentId?.ToString(),
            Children = category.Children
                .Select(MapToCategoryViewModel)
                .ToList(),
            EntryIds = category.Entries
                .Select(ce => ce.EntryId.ToString())
                .ToList()
        };
    }

    private static LabelViewModel MapToLabelViewModel(Label label)
    {
        return new LabelViewModel
        {
            Id = label.Id.ToString(),
            Name = label.Name,
            Description = label.Description,
            Color = label.Color
        };
    }
} 