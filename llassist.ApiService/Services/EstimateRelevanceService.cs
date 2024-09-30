using DotNetWorkQueue;
using llassist.ApiService.Repositories.Specifications;
using llassist.Common;
using llassist.Common.Models;

namespace llassist.ApiService.Services;

public class EstimateRelevanceService
{
    private readonly IProducerQueue<BackgroundTask> _producerQueue;
    private readonly ILogger<EstimateRelevanceService> _logger;
    private readonly ICRUDRepository<Ulid, Project, ProjectSearchSpec> _projectRepository;
    private readonly ICRUDRepository<Ulid, EstimateRelevanceJob, EstimateRelevanceJobSearchSpec> _jobRepository;

    public EstimateRelevanceService(
        IProducerQueue<BackgroundTask> producerQueue,
        ICRUDRepository<Ulid, Project, ProjectSearchSpec> projectRepository,
        ICRUDRepository<Ulid, EstimateRelevanceJob, EstimateRelevanceJobSearchSpec> jobRepository,
        ILogger<EstimateRelevanceService> logger)
    {
        _producerQueue = producerQueue;
        _logger = logger;
        _projectRepository = projectRepository;
        _jobRepository = jobRepository;
    }

    public async Task EnqueuePreprocessingTask(Ulid projectId)
    {
        var project = await _projectRepository.ReadAsync(projectId);
        if (project == null)
        {
            _logger.LogError("Project {projectId} not found", projectId);
            throw new InvalidDataException("Project not found");
        }

        var estimateRelevanceJob = CreateEstimateRelevanceJob(project);

        _logger.LogInformation("Inserting job {jobId} for project {projectId}", estimateRelevanceJob.Id, estimateRelevanceJob.ProjectId);
        var createJobTask = _jobRepository.CreateAsync(estimateRelevanceJob);

        var preprocessingTask = new BackgroundTask
        {
            EstimateRelevanceJobId = estimateRelevanceJob.Id,
            ModelName = "stubbed-value",
            TaskType = TaskType.PREPROCESSING,
            ProjectId = project.Id,
            ResearchQuestions = ToResearchQuestionDTOList(project.ResearchQuestions),
        };

        var enqueueResult = await _producerQueue.SendAsync(preprocessingTask);
        _logger.LogInformation("Preprocessing task enqueue for Project: {projectId}, Job: {jobId} has error: {hasError}", 
            projectId, estimateRelevanceJob.Id, enqueueResult.HasError);

        var createdJob = await createJobTask;
        _logger.LogInformation("Inserted job {jobId} for project {projectId}", createdJob.Id, createdJob.ProjectId);
    }

    private static EstimateRelevanceJob CreateEstimateRelevanceJob(Project project)
    {
        return new EstimateRelevanceJob
        {
            ModelName = "stubbed-value",
            ProjectId = project.Id,
            ResearchQuestions = new ResearchQuestionsSnapshot
            {
                Definitions = project.ResearchQuestions.Definitions,
                Questions = project.ResearchQuestions.Questions.Select(q => new QuestionSnapshot
                {
                    Text = q.Text,
                    Definitions = q.Definitions,
                }).ToList(),
            },
        };
    }

    private static List<ResearchQuestionDTO> ToResearchQuestionDTOList(ResearchQuestions researchQuestions)
    {
        var researchQuestionList = new List<ResearchQuestionDTO>();
        foreach (var question in researchQuestions.Questions)
        {
            researchQuestionList.Add(new ResearchQuestionDTO
            {
                Question = question.Text,
                CombinedDefinitions = researchQuestions.Definitions.Concat(question.Definitions).ToArray(),
            });
        }
        return researchQuestionList;
    }

    private async Task<EstimateRelevanceJob> InsertEstimateRelevanceJob(EstimateRelevanceJob job)
    {
        _logger.LogInformation("Inserting job {jobId} for project {projectId}", job.Id, job.ProjectId);
        return await _jobRepository.CreateAsync(job);
    }
}
