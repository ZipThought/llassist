using llassist.Common.ViewModels;

namespace llassist.Web;

public class LibraryApiClient
{
    private readonly HttpClient _httpClient;

    public LibraryApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<CatalogViewModel>> GetAllCatalogsAsync()
    {
        var catalogs = await _httpClient.GetFromJsonAsync<IEnumerable<CatalogViewModel>>("api/library/catalogs");
        return catalogs ?? Enumerable.Empty<CatalogViewModel>();
    }

    public async Task<CatalogViewModel?> GetCatalogAsync(string id)
    {
        return await _httpClient.GetFromJsonAsync<CatalogViewModel>($"api/library/catalogs/{id}");
    }

    public async Task<CatalogViewModel?> CreateCatalogAsync(string name, string description, string owner)
    {
        var response = await _httpClient.PostAsJsonAsync("api/library/catalogs", new { name, description, owner });
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<CatalogViewModel>();
    }

    public async Task DeleteCatalogAsync(string id)
    {
        var response = await _httpClient.DeleteAsync($"api/library/catalogs/{id}");
        response.EnsureSuccessStatusCode();
    }

    public async Task<bool> DeleteEntryAsync(string id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/library/entries/{id}");
            response.EnsureSuccessStatusCode();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting entry: {ex.Message}");
            throw;
        }
    }

    public async Task<CatalogViewModel?> UpdateCatalogAsync(string id, string name, string description)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/library/catalogs/{id}", new { name, description });
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<CatalogViewModel>();
    }

    public async Task<EntryViewModel?> GetEntryAsync(string id)
    {
        return await _httpClient.GetFromJsonAsync<EntryViewModel>($"api/library/entries/{id}");
    }

    public async Task<EntryViewModel?> CreateEntryAsync(string catalogId, CreateEditEntryViewModel entry)
    {
        var response = await _httpClient.PostAsJsonAsync($"api/library/catalogs/{catalogId}/entries", entry);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<EntryViewModel>();
    }

    public async Task<EntryViewModel?> UpdateEntryAsync(string id, CreateEditEntryViewModel entry)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"api/library/entries/{id}", entry);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<EntryViewModel>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating entry: {ex.Message}");
            throw;
        }
    }

    public async Task<CategoryTreeViewModel> GetCategoryTreeAsync(string catalogId, string schemaType)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<CategoryTreeViewModel>(
                $"api/library/catalogs/{catalogId}/categories?schemaType={Uri.EscapeDataString(schemaType)}");
            return response ?? new CategoryTreeViewModel { SchemaType = schemaType };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting category tree: {ex.Message}");
            throw;
        }
    }

    public async Task<CategoryViewModel> CreateCategoryAsync(string catalogId, CategoryViewModel category)
    {
        var response = await _httpClient.PostAsJsonAsync(
            $"api/library/catalogs/{catalogId}/categories", category);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<CategoryViewModel>()
            ?? throw new InvalidOperationException("Failed to create category");
    }

    public async Task<CategoryViewModel> UpdateCategoryAsync(string id, CategoryViewModel category)
    {
        var response = await _httpClient.PutAsJsonAsync(
            $"api/library/categories/{id}", category);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<CategoryViewModel>()
            ?? throw new InvalidOperationException("Failed to update category");
    }

    public async Task DeleteCategoryAsync(string id)
    {
        var response = await _httpClient.DeleteAsync($"api/library/categories/{id}");
        response.EnsureSuccessStatusCode();
    }

    public async Task<CategoryViewModel?> GetCategoryAsync(string id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<CategoryViewModel>($"api/library/categories/{id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting category: {ex.Message}");
            throw;
        }
    }

    public async Task<IEnumerable<EntryViewModel>> GetEntriesByCategoryAsync(string categoryId)
    {
        try
        {
            var entries = await _httpClient.GetFromJsonAsync<IEnumerable<EntryViewModel>>(
                $"api/library/categories/{categoryId}/entries");
            return entries ?? Enumerable.Empty<EntryViewModel>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting entries by category: {ex.Message}");
            throw;
        }
    }
} 