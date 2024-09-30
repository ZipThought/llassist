using DotNetWorkQueue;
using llassist.ApiService.Repositories.Specifications;
using llassist.ApiService.Services;
using llassist.Common.Models;
using llassist.Common;
using Microsoft.Extensions.Logging;
using Xunit;
using Moq;
using Assert = Xunit.Assert;
using DotNetWorkQueue.Messages;

namespace llassist.Tests;

public class EstimateRelevanceServiceTests
{
    private readonly Mock<IProducerQueue<BackgroundTask>> _mockProducerQueue;
    private readonly Mock<ILogger<EstimateRelevanceService>> _mockLogger;
    private readonly Mock<ICRUDRepository<Ulid, Project, ProjectSearchSpec>> _mockProjectRepository;
    private readonly Mock<ICRUDRepository<Ulid, EstimateRelevanceJob, EstimateRelevanceJobSearchSpec>> _mockJobRepository;

    public EstimateRelevanceServiceTests()
    {
        _mockProducerQueue = new Mock<IProducerQueue<BackgroundTask>>();
        _mockLogger = new Mock<ILogger<EstimateRelevanceService>>();
        _mockProjectRepository = new Mock<ICRUDRepository<Ulid, Project, ProjectSearchSpec>>();
        _mockJobRepository = new Mock<ICRUDRepository<Ulid, EstimateRelevanceJob, EstimateRelevanceJobSearchSpec>>();
    }

    [Fact]
    public async Task EnqueuePreprocessingTask_ShouldEnqueueTaskAndCreateJob_WhenProjectExists()
    {
        // Arrange
        var projectId = Ulid.NewUlid();
        var project = CreateSampleProject(projectId);
        var estimateRelevanceJob = CreateSampleEstimateRelevanceJob(project);

        _mockProjectRepository.Setup(r => r.ReadAsync(projectId)).ReturnsAsync(project);
        _mockJobRepository.Setup(r => r.CreateAsync(It.IsAny<EstimateRelevanceJob>())).ReturnsAsync(estimateRelevanceJob);
        _mockProducerQueue.Setup(q => q.SendAsync(It.IsAny<BackgroundTask>(), null))
            .ReturnsAsync(new QueueOutputMessage(It.IsAny<ISentMessage>(), null));

        var service = new EstimateRelevanceService(_mockProducerQueue.Object, _mockProjectRepository.Object,
            _mockJobRepository.Object, _mockLogger.Object);

        // Act
        await service.EnqueuePreprocessingTask(projectId);

        // Assert
        _mockProjectRepository.Verify(r => r.ReadAsync(projectId), Times.Once);
        _mockProducerQueue.Verify(q => q.SendAsync(It.IsAny<BackgroundTask>(), null), Times.Once);
        _mockJobRepository.Verify(r => r.CreateAsync(It.IsAny<EstimateRelevanceJob>()), Times.Once);
    }

    [Fact]
    public async Task EnqueuePreprocessingTask_ShouldThrowException_WhenProjectNotFound()
    {
        // Arrange
        var projectId = Ulid.NewUlid();
        _mockProjectRepository.Setup(r => r.ReadAsync(projectId)).ReturnsAsync((Project)null);

        var service = new EstimateRelevanceService(_mockProducerQueue.Object, _mockProjectRepository.Object,
            _mockJobRepository.Object, _mockLogger.Object);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidDataException>(() => service.EnqueuePreprocessingTask(projectId));
    }

    private static Project CreateSampleProject(Ulid projectId)
    {
        return new Project
        {
            Id = projectId,
            ResearchQuestions = new ResearchQuestions
            {
                Definitions = new List<string> { "Definition 1", "Definition 2" },
                Questions = new List<Question>
                {
                    new Question { Text = "Question 1", Definitions = new List<string> { "Q1 Definition" } },
                    new Question { Text = "Question 2", Definitions = new List<string> { "Q2 Definition" } }
                }
            }
        };
    }

    private static EstimateRelevanceJob CreateSampleEstimateRelevanceJob(Project project)
    {
        return new EstimateRelevanceJob
        {
            Id = Ulid.NewUlid(),
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
}