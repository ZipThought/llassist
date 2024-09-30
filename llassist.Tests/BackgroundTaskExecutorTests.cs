using DotNetWorkQueue;
using llassist.ApiService.Executors;
using llassist.ApiService.Repositories.Specifications;
using llassist.ApiService.Services;
using llassist.Common.Models;
using llassist.Common;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Assert = Xunit.Assert;
using System.Threading.Tasks;

namespace llassist.Tests;

public class BackgroundTaskExecutorTests
{
    private readonly Mock<IServiceScopeFactory> _mockServiceScopeFactory;
    private readonly Mock<IConsumerQueue> _mockConsumerQueue;
    private readonly Mock<IProducerQueue<BackgroundTask>> _mockProducerQueue;
    private readonly Mock<ILogger<BackgroundTaskExecutor>> _mockLogger;
    private readonly Mock<ICRUDRepository<Ulid, Article, ArticleSearchSpec>> _mockArticleRepository;
    private readonly Mock<INLPService> _mockNlpService;

    public BackgroundTaskExecutorTests()
    {
        _mockServiceScopeFactory = new Mock<IServiceScopeFactory>();
        _mockConsumerQueue = new Mock<IConsumerQueue>();
        _mockProducerQueue = new Mock<IProducerQueue<BackgroundTask>>();
        _mockLogger = new Mock<ILogger<BackgroundTaskExecutor>>();
        _mockArticleRepository = new Mock<ICRUDRepository<Ulid, Article, ArticleSearchSpec>>();
        _mockNlpService = new Mock<INLPService>();

        SetupMockServiceProvider();
    }

    private void SetupMockServiceProvider()
    {
        var mockServiceProvider = new Mock<IServiceProvider>();
        mockServiceProvider
            .Setup(x => x.GetService(typeof(ICRUDRepository<Ulid, Article, ArticleSearchSpec>)))
            .Returns(_mockArticleRepository.Object);
        mockServiceProvider
            .Setup(x => x.GetService(typeof(INLPService)))
            .Returns(_mockNlpService.Object);

        var mockServiceScope = new Mock<IServiceScope>();
        mockServiceScope.Setup(x => x.ServiceProvider).Returns(mockServiceProvider.Object);

        _mockServiceScopeFactory
            .Setup(x => x.CreateScope())
            .Returns(mockServiceScope.Object);
    }

    [Fact]
    public async Task HandleMessages_PreprocessingTask_ShouldExecutePreprocessingTask()
    {
        // Arrange
        var executor = CreateBackgroundTaskExecutor();
        var task = new BackgroundTask { TaskType = TaskType.PREPROCESSING, ProjectId = Ulid.NewUlid() };
        var message = CreateMockReceivedMessage(task);
        var notifications = Mock.Of<IWorkerNotification>();

        var articles = new List<Article> { new Article(), new Article() };
        _mockArticleRepository
            .Setup(x => x.ReadWithSearchSpecAsync(It.IsAny<ArticleSearchSpec>()))
            .ReturnsAsync(articles);

        _mockProducerQueue
            .Setup(x => x.SendAsync(It.IsAny<List<BackgroundTask>>()))
            .ReturnsAsync(Mock.Of<IQueueOutputMessages>());

        // Act
        await Task.Run(() => executor.HandleMessages(message, notifications));

        // Assert
        _mockProducerQueue.Verify(
            x => x.SendAsync(It.Is<List<BackgroundTask>>(tasks => tasks.Count == 2 && tasks.All(t => t.TaskType == TaskType.EXECUTION))),
            Times.Once
        );
    }

    [Fact]
    public async Task HandleMessages_ExecutionTask_ShouldExecuteExecutionTask()
    {
        // Arrange
        var executor = CreateBackgroundTaskExecutor();
        var articleId = Ulid.NewUlid();
        var task = new BackgroundTask
        {
            TaskType = TaskType.EXECUTION,
            ArticleId = articleId,
            ResearchQuestions = new List<ResearchQuestionDTO> {
                new ResearchQuestionDTO { Question = "Test Question", CombinedDefinitions = [ "Test-Definition" ] }
            }
        };
        var message = CreateMockReceivedMessage(task);
        var notifications = Mock.Of<IWorkerNotification>();

        var article = new Article { Id = articleId };
        _mockArticleRepository
            .Setup(x => x.ReadAsync(articleId))
            .ReturnsAsync(article);

        _mockNlpService
            .Setup(x => x.ExtractKeySemantics(It.IsAny<string>()))
            .ReturnsAsync(new KeySemantics());

        _mockNlpService
            .Setup(x => x.EstimateRevelance(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string[]>()))
            .ReturnsAsync(new Relevance { Question = "Test Question" });

        // Act
        await Task.Run(() => executor.HandleMessages(message, notifications));

        // Assert
        Assert.Equal(task.ResearchQuestions.Count, article.Relevances.Count);
        Assert.Equal("Test Question", article.Relevances[0].Question);
        _mockArticleRepository.Verify(x => x.UpdateAsync(It.IsAny<Article>()), Times.Once);
        _mockNlpService.Verify(x => x.ExtractKeySemantics(It.IsAny<string>()), Times.Once);
        _mockNlpService.Verify(x => x.EstimateRevelance(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string[]>()), Times.Once);
    }

    [Fact]
    public async Task HandleMessages_ExecutionTask_AlreadyProcessed_ShouldNotProcessAndUpdateArticle()
    {
        // Arrange
        var executor = CreateBackgroundTaskExecutor();
        var articleId = Ulid.NewUlid();
        var jobId = Ulid.NewUlid();
        var task = new BackgroundTask
        {
            TaskType = TaskType.EXECUTION,
            ArticleId = articleId,
            EstimateRelevanceJobId = jobId,
            ResearchQuestions = new List<ResearchQuestionDTO> {
                new ResearchQuestionDTO { Question = "Test Question", CombinedDefinitions = [ "Test-Definition" ] }
            }
        };
        var message = CreateMockReceivedMessage(task);
        var notifications = Mock.Of<IWorkerNotification>();

        var article = new Article { Id = articleId, EstimateRelevanceJobId = jobId };
        _mockArticleRepository
            .Setup(x => x.ReadAsync(articleId))
            .ReturnsAsync(article);

        // Act
        await Task.Run(() => executor.HandleMessages(message, notifications));

        // Assert
        _mockNlpService.Verify(x => x.ExtractKeySemantics(It.IsAny<string>()), Times.Never);
        _mockNlpService.Verify(x => x.EstimateRevelance(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string[]>()), Times.Never);
        _mockArticleRepository.Verify(x => x.UpdateAsync(It.IsAny<Article>()), Times.Never);
    }

    [Fact]
    public async Task HandleMessages_ExecutionTask_ArticleNotFound_ShouldNotProcessAndUpdateArticle()
    {
        // Arrange
        var executor = CreateBackgroundTaskExecutor();
        var articleId = Ulid.NewUlid();
        var task = new BackgroundTask
        {
            TaskType = TaskType.EXECUTION,
            ArticleId = articleId,
        };
        var message = CreateMockReceivedMessage(task);
        var notifications = Mock.Of<IWorkerNotification>();

        _mockArticleRepository
            .Setup(x => x.ReadAsync(articleId))
            .ReturnsAsync((Article)null);

        // Act
        await Task.Run(() => executor.HandleMessages(message, notifications));

        // Assert
        _mockNlpService.Verify(x => x.ExtractKeySemantics(It.IsAny<string>()), Times.Never);
        _mockNlpService.Verify(x => x.EstimateRevelance(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string[]>()), Times.Never);
        _mockArticleRepository.Verify(x => x.UpdateAsync(It.IsAny<Article>()), Times.Never);
    }

    [Fact]
    public void HandleMessages_InvalidTaskType_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var executor = CreateBackgroundTaskExecutor();
        var task = new BackgroundTask { TaskType = (TaskType)999 }; // Invalid TaskType
        var message = CreateMockReceivedMessage(task);
        var notifications = Mock.Of<IWorkerNotification>();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => executor.HandleMessages(message, notifications));
    }

    private BackgroundTaskExecutor CreateBackgroundTaskExecutor()
    {
        return new BackgroundTaskExecutor(
            _mockServiceScopeFactory.Object,
            _mockConsumerQueue.Object,
            _mockLogger.Object,
            _mockProducerQueue.Object
        );
    }

    private static IReceivedMessage<BackgroundTask> CreateMockReceivedMessage(BackgroundTask task)
    {
        var mockMessage = new Mock<IReceivedMessage<BackgroundTask>>();
        mockMessage.Setup(x => x.Body).Returns(task);
        return mockMessage.Object;
    }
}