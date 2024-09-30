using llassist.ApiService.Repositories.Specifications;
using llassist.Common;
using llassist.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace llassist.ApiService.Repositories;

public class EstimateRelevanceJobRepository : ICRUDRepository<Ulid, EstimateRelevanceJob, EstimateRelevanceJobSearchSpec>
{
    private readonly ApplicationDbContext _context;

    public EstimateRelevanceJobRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<EstimateRelevanceJob> CreateAsync(EstimateRelevanceJob job)
    {
        _context.EstimateRelevanceJobs.Add(job);
        await _context.SaveChangesAsync();
        return job;
    }

    public async Task<bool> DeleteAsync(Ulid id)
    {
        var job = _context.EstimateRelevanceJobs.Find(id);
        if (job == null)
        {
            return false;   
        }

        _context.EstimateRelevanceJobs.Remove(job);
        await _context.SaveChangesAsync();
        return true;
    }

    public Task<IEnumerable<EstimateRelevanceJob>> ReadAllAsync()
    {
        throw new NotImplementedException("This method is not supported for EstimateRelevanceJobs");
    }

    public async Task<EstimateRelevanceJob?> ReadAsync(Ulid id)
    {
        return await _context.EstimateRelevanceJobs.FindAsync(id);
    }

    public async Task<IEnumerable<EstimateRelevanceJob>> ReadWithSearchSpecAsync(EstimateRelevanceJobSearchSpec searchSpec)
    {
        return await _context.EstimateRelevanceJobs
            .Where(j => j.ProjectId == searchSpec.ProjectId)
            .ToListAsync();
    }

    public async Task<EstimateRelevanceJob> UpdateAsync(EstimateRelevanceJob job)
    {
        _context.Entry(job).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return job;
    }
}
