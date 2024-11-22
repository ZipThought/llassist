using Microsoft.EntityFrameworkCore;
using llassist.Common.Models.Library;
using llassist.ApiService.Repositories.Specifications;

namespace llassist.ApiService.Repositories;

public class LibraryRepository : CRUDBaseRepository<Ulid, Entry, LibrarySearchSpec>
{
    public LibraryRepository(ApplicationDbContext context) : base(context) { }

    public override DbSet<Entry> GetDbSet()
    {
        return _context.Entries;
    }

    public override async Task<Entry?> ReadAsync(Ulid id)
    {
        return await GetDbSet()
            .Include(e => e.Resources)
            .Include(e => e.Labels)
                .ThenInclude(el => el.Label)
            .Include(e => e.Categories)
                .ThenInclude(ce => ce.Category)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public override async Task<IEnumerable<Entry>> ReadWithSearchSpecAsync(LibrarySearchSpec spec)
    {
        var query = GetDbSet()
            .Include(e => e.Resources)
            .Include(e => e.Labels)
                .ThenInclude(el => el.Label)
            .Include(e => e.Categories)
                .ThenInclude(ce => ce.Category)
            .AsQueryable();

        if (spec.CatalogId.HasValue)
            query = query.Where(e => e.CatalogId == spec.CatalogId);

        if (!string.IsNullOrEmpty(spec.SearchTerm))
            query = query.Where(e => 
                e.Title.Contains(spec.SearchTerm) || 
                e.Description.Contains(spec.SearchTerm));

        if (!string.IsNullOrEmpty(spec.EntryType))
            query = query.Where(e => e.EntryType == spec.EntryType);

        if (spec.FromDate.HasValue)
            query = query.Where(e => e.CreatedAt >= spec.FromDate);

        if (spec.ToDate.HasValue)
            query = query.Where(e => e.CreatedAt <= spec.ToDate);

        if (spec.LabelIds.Any())
            query = query.Where(e => e.Labels.Any(el => 
                spec.LabelIds.Contains(el.LabelId)));

        if (spec.CategoryIds.Any())
            query = query.Where(e => e.Categories.Any(ce => 
                spec.CategoryIds.Contains(ce.CategoryId)));

        return await query.ToListAsync();
    }
} 