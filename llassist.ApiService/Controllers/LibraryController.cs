using Microsoft.AspNetCore.Mvc;
using llassist.Common.ViewModels;
using llassist.ApiService.Services;

namespace llassist.ApiService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LibraryController : ControllerBase
{
    private readonly ILibraryService _libraryService;
    private readonly ILogger<LibraryController> _logger;

    public LibraryController(ILibraryService libraryService, ILogger<LibraryController> logger)
    {
        _libraryService = libraryService;
        _logger = logger;
    }

    [HttpPost("catalogs")]
    public async Task<ActionResult<CatalogViewModel>> CreateCatalog([FromBody] CreateCatalogRequest request)
    {
        try
        {
            var catalog = await _libraryService.CreateCatalogAsync(request.Name, request.Description, request.Owner);
            return CreatedAtAction(nameof(GetCatalog), new { id = catalog.Id }, catalog);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating catalog");
            return StatusCode(500, "An error occurred while creating the catalog");
        }
    }

    [HttpGet("catalogs/{id}")]
    public async Task<ActionResult<CatalogViewModel>> GetCatalog(string id)
    {
        var catalog = await _libraryService.GetCatalogAsync(Ulid.Parse(id));
        if (catalog == null)
        {
            return NotFound();
        }
        return Ok(catalog);
    }

    [HttpGet("catalogs")]
    public async Task<ActionResult<IEnumerable<CatalogViewModel>>> GetAllCatalogs()
    {
        var catalogs = await _libraryService.GetAllCatalogsAsync();
        return Ok(catalogs);
    }

    [HttpPut("catalogs/{id}")]
    public async Task<ActionResult<CatalogViewModel>> UpdateCatalog(string id, [FromBody] UpdateCatalogRequest request)
    {
        var catalog = await _libraryService.UpdateCatalogAsync(Ulid.Parse(id), request.Name, request.Description);
        if (catalog == null)
        {
            return NotFound();
        }
        return Ok(catalog);
    }

    [HttpDelete("catalogs/{id}")]
    public async Task<IActionResult> DeleteCatalog(string id)
    {
        var result = await _libraryService.DeleteCatalogAsync(Ulid.Parse(id));
        if (!result)
        {
            return NotFound();
        }
        return NoContent();
    }

    [HttpPost("catalogs/{catalogId}/entries")]
    public async Task<ActionResult<EntryViewModel>> CreateEntry(string catalogId, CreateEditEntryViewModel entry)
    {
        try
        {
            var ulid = Ulid.Parse(catalogId);
            var created = await _libraryService.CreateEntryAsync(ulid, entry);
            return created != null ? Ok(created) : BadRequest();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating entry");
            return StatusCode(500, "An error occurred while creating the entry");
        }
    }

    [HttpGet("entries/{id}")]
    public async Task<ActionResult<EntryViewModel>> GetEntry(string id)
    {
        try
        {
            var ulid = Ulid.Parse(id);
            var entry = await _libraryService.GetEntryAsync(ulid);
            return entry != null ? Ok(entry) : NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting entry");
            return StatusCode(500, "An error occurred while getting the entry");
        }
    }

    [HttpGet("catalogs/{catalogId}/entries")]
    public async Task<ActionResult<IEnumerable<EntryViewModel>>> GetEntriesByCatalog(string catalogId)
    {
        var entries = await _libraryService.GetEntriesByCatalogAsync(Ulid.Parse(catalogId));
        return Ok(entries);
    }

    [HttpPost("catalogs/{catalogId}/categories")]
    public async Task<ActionResult<CategoryViewModel>> CreateCategory(string catalogId, [FromBody] CreateCategoryRequest request)
    {
        try
        {
            var category = await _libraryService.CreateCategoryAsync(
                Ulid.Parse(catalogId),
                new CategoryViewModel 
                { 
                    Name = request.Name,
                    Description = request.Description,
                    SchemaType = request.SchemaType,
                    ParentId = request.ParentId
                });
            return CreatedAtAction(nameof(GetCategoryTree), new { catalogId, schemaType = category.SchemaType }, category);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating category");
            return StatusCode(500, "An error occurred while creating the category");
        }
    }

    [HttpGet("catalogs/{catalogId}/categories")]
    public async Task<ActionResult<CategoryTreeViewModel>> GetCategoryTree(string catalogId, [FromQuery] string? schemaType)
    {
        try
        {
            if (string.IsNullOrEmpty(catalogId))
            {
                return BadRequest("Catalog ID is required");
            }

            var tree = await _libraryService.GetCategoryTreeAsync(
                Ulid.Parse(catalogId), 
                schemaType ?? "subject" // default schema type if none provided
            );
            return Ok(tree);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting category tree for catalog {CatalogId}", catalogId);
            return StatusCode(500, "An error occurred while getting the category tree");
        }
    }

    [HttpPost("entries/{entryId}/categories/{categoryId}")]
    public async Task<IActionResult> AssignCategory(string entryId, string categoryId)
    {
        var result = await _libraryService.AssignEntryToCategoryAsync(
            Ulid.Parse(entryId), 
            Ulid.Parse(categoryId));
        
        if (!result) return NotFound();
        return NoContent();
    }

    [HttpDelete("entries/{entryId}/categories/{categoryId}")]
    public async Task<IActionResult> RemoveCategory(string entryId, string categoryId)
    {
        var result = await _libraryService.RemoveEntryFromCategoryAsync(
            Ulid.Parse(entryId), 
            Ulid.Parse(categoryId));
        
        if (!result) return NotFound();
        return NoContent();
    }

    [HttpPost("catalogs/{catalogId}/labels")]
    public async Task<ActionResult<LabelViewModel>> CreateLabel(string catalogId, [FromBody] CreateLabelRequest request)
    {
        try
        {
            var label = await _libraryService.CreateLabelAsync(
                Ulid.Parse(catalogId),
                new LabelViewModel 
                { 
                    Name = request.Name,
                    Description = request.Description,
                    Color = request.Color
                });
            return CreatedAtAction(nameof(GetEntry), new { id = label.Id }, label);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating label");
            return StatusCode(500, "An error occurred while creating the label");
        }
    }

    [HttpPost("entries/{entryId}/labels/{labelId}")]
    public async Task<IActionResult> AssignLabel(string entryId, string labelId)
    {
        var result = await _libraryService.AssignLabelToEntryAsync(
            Ulid.Parse(entryId), 
            Ulid.Parse(labelId));
        
        if (!result) return NotFound();
        return NoContent();
    }

    [HttpDelete("entries/{entryId}/labels/{labelId}")]
    public async Task<IActionResult> RemoveLabel(string entryId, string labelId)
    {
        var result = await _libraryService.RemoveLabelFromEntryAsync(
            Ulid.Parse(entryId), 
            Ulid.Parse(labelId));
        
        if (!result) return NotFound();
        return NoContent();
    }

    [HttpPut("entries/{id}")]
    public async Task<ActionResult<EntryViewModel>> UpdateEntry(string id, CreateEditEntryViewModel entry)
    {
        try
        {
            var ulid = Ulid.Parse(id);
            var updated = await _libraryService.UpdateEntryAsync(ulid, entry);
            return updated != null ? Ok(updated) : NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating entry");
            return StatusCode(500, "An error occurred while updating the entry");
        }
    }

    [HttpDelete("entries/{id}")]
    public async Task<IActionResult> DeleteEntry(string id)
    {
        try
        {
            var ulid = Ulid.Parse(id);
            var success = await _libraryService.DeleteEntryAsync(ulid);
            return success ? Ok() : NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting entry");
            return StatusCode(500, "An error occurred while deleting the entry");
        }
    }

    [HttpGet("categories/{id}")]
    public async Task<ActionResult<CategoryViewModel>> GetCategory(string id)
    {
        try
        {
            var category = await _libraryService.GetCategoryAsync(Ulid.Parse(id));
            if (category == null)
            {
                return NotFound();
            }
            return Ok(category);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting category");
            return StatusCode(500, "An error occurred while getting the category");
        }
    }

    [HttpGet("categories/{categoryId}/entries")]
    public async Task<ActionResult<IEnumerable<EntryViewModel>>> GetEntriesByCategory(string categoryId)
    {
        try
        {
            var entries = await _libraryService.GetEntriesByCategoryAsync(Ulid.Parse(categoryId));
            return Ok(entries);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting entries by category");
            return StatusCode(500, "An error occurred while getting entries");
        }
    }

    [HttpDelete("categories/{id}")]
    public async Task<IActionResult> DeleteCategory(string id)
    {
        try
        {
            var result = await _libraryService.DeleteCategoryAsync(Ulid.Parse(id));
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting category");
            return StatusCode(500, "An error occurred while deleting the category");
        }
    }

    [HttpPut("categories/{id}")]
    public async Task<ActionResult<CategoryViewModel>> UpdateCategory(string id, [FromBody] CategoryViewModel category)
    {
        try
        {
            var updated = await _libraryService.UpdateCategoryAsync(Ulid.Parse(id), category);
            if (updated == null)
            {
                return NotFound();
            }
            return Ok(updated);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating category");
            return StatusCode(500, "An error occurred while updating the category");
        }
    }

    // Additional request models
    public class CreateCatalogRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Owner { get; set; } = string.Empty;
    }

    public class UpdateCatalogRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class CreateCategoryRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string SchemaType { get; set; } = string.Empty;
        public string? ParentId { get; set; }
    }

    public class CreateLabelRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
    }
} 