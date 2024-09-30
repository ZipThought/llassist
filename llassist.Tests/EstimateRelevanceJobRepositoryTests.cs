using llassist.ApiService.Repositories;
using llassist.ApiService.Repositories.Specifications;
using llassist.Common.Models;
using Xunit;
using Assert = Xunit.Assert;

namespace llassist.Tests;

public class EstimateRelevanceJobRepositoryTests : IClassFixture<DatabaseFixture>, IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly EstimateRelevanceJobRepository _repository;

    private readonly EstimateRelevanceJob EstimateRelevanceJob;

    public EstimateRelevanceJobRepositoryTests(DatabaseFixture fixture)
    {
        _context = fixture.CreateContext();
        _repository = new EstimateRelevanceJobRepository(_context);

        var project = fixture.Project;
        Assert.NotNull(project);

        EstimateRelevanceJob = new EstimateRelevanceJob
        {
            Id = Ulid.NewUlid(),
            ModelName = "model-name",
            ProjectId = project.Id,
            CreatedAt = DateTime.UtcNow,
            ResearchQuestions = new ResearchQuestionsSnapshot
            {
                Definitions = ["definitions-1", "definitions-2"],
                Questions =
                [
                    new() {
                        Text = "text-1",
                        Definitions = ["definitions-5", "definitions-6"]
                    },
                    new() {
                        Text = "text-2",
                        Definitions = ["definitions-3", "definitions-4"]
                    }
                ]
            }
        };
    }

    [Fact]
    public async Task CRUDJob()
    {
        // Create then read
        await _repository.CreateAsync(EstimateRelevanceJob);

        var readJob = await _repository.ReadAsync(EstimateRelevanceJob.Id);
        VerifyJob(readJob);

        // Update then find from read-with-search-spec
        EstimateRelevanceJob.ModelName = "Updated model-name";
        await _repository.UpdateAsync(EstimateRelevanceJob);

        var jobsByProjectId = await _repository.ReadWithSearchSpecAsync(
            new EstimateRelevanceJobSearchSpec
            {
                ProjectId = EstimateRelevanceJob.ProjectId
            });
        EstimateRelevanceJob? foundJob = null;
        foreach (var job in jobsByProjectId)
        {
            if (job.Id == EstimateRelevanceJob.Id)
            {
                foundJob = job;
            }
        }
        VerifyJob(foundJob);

        // Delete then read
        await _repository.DeleteAsync(EstimateRelevanceJob.Id);
        readJob = await _repository.ReadAsync(EstimateRelevanceJob.Id);
        Assert.Null(readJob);
    }

    private void VerifyJob(EstimateRelevanceJob? job)
    {
        Assert.NotNull(job);
        Assert.Equal(job.ModelName, EstimateRelevanceJob.ModelName);
        Assert.Equal(job.ProjectId, EstimateRelevanceJob.ProjectId);
        Assert.Equal(job.CreatedAt, EstimateRelevanceJob.CreatedAt);
        Assert.Equal(job.ResearchQuestions.Definitions, EstimateRelevanceJob.ResearchQuestions.Definitions);
        Assert.Equal(job.ResearchQuestions.Questions.Count, EstimateRelevanceJob.ResearchQuestions.Questions.Count);
        for (var i = 0; i < job.ResearchQuestions.Questions.Count; i++)
        {
            Assert.Equal(job.ResearchQuestions.Questions[i].Text, EstimateRelevanceJob.ResearchQuestions.Questions[i].Text);
            Assert.Equal(job.ResearchQuestions.Questions[i].Definitions, EstimateRelevanceJob.ResearchQuestions.Questions[i].Definitions);
        }
    }

    public void Dispose()
    {
        try
        {
            _context.EstimateRelevanceJobs.Remove(EstimateRelevanceJob);
            _context.SaveChanges();
        }
        catch (Exception)
        {
            // Ignore the error if the row is already removed
        }
    }
}
