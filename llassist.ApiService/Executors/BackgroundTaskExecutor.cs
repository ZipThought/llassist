using DotNetWorkQueue;
using llassist.ApiService.Repositories.Specifications;
using llassist.ApiService.Services;
using llassist.Common;
using llassist.Common.Models;
using System.Text.Json;

namespace llassist.ApiService.Executors;

public class BackgroundTaskExecutor
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IConsumerQueue _consumerQueue;
    private readonly IProducerQueue<BackgroundTask> _producerQueue;
    private readonly ILogger<BackgroundTaskExecutor> _logger;

    public BackgroundTaskExecutor(
        IServiceScopeFactory serviceScopeFactory,
        IConsumerQueue consumerQueue,
        ILogger<BackgroundTaskExecutor> logger,
        IProducerQueue<BackgroundTask> producerQueue)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _consumerQueue = consumerQueue;
        _producerQueue = producerQueue;
        _logger = logger;

        _consumerQueue.Start<BackgroundTask>(HandleMessages, CreateNotifications.Create(_logger));
    }

    public void HandleMessages(IReceivedMessage<BackgroundTask> message, IWorkerNotification notifications)
    {
        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            switch (message.Body.TaskType)
            {
                case TaskType.PREPROCESSING:
                    ExecutePreprocessingTask(message.Body, scope).Wait();
                    break;
                case TaskType.EXECUTION:
                    ExecuteExecutionTask(message.Body, scope).Wait();
                    break;
                default:
                    throw new InvalidOperationException("Invalid TaskType value");
            }
        } 
        catch(Exception ex)
        {
            _logger.LogError(ex, "Failed to handle message: {message}", JsonSerializer.Serialize(message));

            if (ex is InvalidOperationException)
            {
                throw;
            }
            else
            {
                throw new InvalidDataException("Failed to handle message", ex);
            }
        }

    }

    private async Task ExecutePreprocessingTask(BackgroundTask task, IServiceScope scope)
    {
        _logger.LogInformation("Executing PREPROCESSING task {task}", JsonSerializer.Serialize(task));

        var articleRepository = scope.ServiceProvider.GetRequiredService<ICRUDRepository<Ulid, Article, ArticleSearchSpec>>();

        var articleList = await articleRepository.ReadWithSearchSpecAsync(
            new ArticleSearchSpec
            {
                ProjectId = task.ProjectId
            });

        await _producerQueue.SendAsync(CreateExecutionTasks(task, articleList));
    }

    private static List<BackgroundTask> CreateExecutionTasks(BackgroundTask preprocessingTask, IEnumerable<Article> articles)
    {
        var executionTasks = new List<BackgroundTask>();
        foreach (var article in articles)
        {
            executionTasks.Add(new BackgroundTask
            {
                EstimateRelevanceJobId = preprocessingTask.EstimateRelevanceJobId,
                ModelName = preprocessingTask.ModelName,
                TaskType = TaskType.EXECUTION,
                ProjectId = preprocessingTask.ProjectId,
                ArticleId = article.Id,
                ResearchQuestions = preprocessingTask.ResearchQuestions,
            });
        }

        return executionTasks;
    }

    private async Task ExecuteExecutionTask(BackgroundTask task, IServiceScope scope)
    {
        _logger.LogInformation("Executing EXECUTION task {task}", JsonSerializer.Serialize(task));

        var articleRepository = scope.ServiceProvider.GetRequiredService<ICRUDRepository<Ulid, Article, ArticleSearchSpec>>();
        var nlpService = scope.ServiceProvider.GetRequiredService<INLPService>();

        var article = await articleRepository.ReadAsync(task.ArticleId);
        if (article == null)
        {
            _logger.LogError("Failed to find article with id: {articleId}", task.ArticleId);
            return;
        }
        else if (article.EstimateRelevanceJobId != null && article.EstimateRelevanceJobId == task.EstimateRelevanceJobId)
        {
            _logger.LogInformation("Article {articleId} already processed with JobId: {jobId}", 
                article.Id, article.EstimateRelevanceJobId);
            return;
        }

        var mustRead = false;
        article.Relevances = new List<Relevance>();
        article.KeySemantics = await ExtractKeySemanticsFromArticle(article, nlpService);

        foreach (var researchQuestion in task.ResearchQuestions)
        {
            var relevance = await EstimateRelevance(article, researchQuestion, nlpService);
            article.Relevances.Add(relevance);
            
            mustRead = mustRead || relevance.IsRelevant || relevance.IsContributing;
        }

        article.MustRead = mustRead;
        article.EstimateRelevanceJobId = task.EstimateRelevanceJobId;
        await articleRepository.UpdateAsync(article);

        _logger.LogInformation("Updated article {articleId} with MustRead: {mustRead} and JobId: {jobId}",
            article.Id, mustRead, article.EstimateRelevanceJobId);
    }

    private async Task<KeySemantics> ExtractKeySemanticsFromArticle(Article article, INLPService nlpService)
    {
        try
        {
            _logger.LogInformation("Extracting semantics for Article: {articleId}", article.Id);

            return await nlpService.ExtractKeySemantics($"Title: {article.Title}\n Abstract: {article.Abstract}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to extract semantics for Article: {articleId}", article.Id);
            throw new InvalidOperationException("Failed to extract semantics for Article", ex);
        }
    }

    private async Task<Relevance> EstimateRelevance(Article article, ResearchQuestionDTO researchQuestion, INLPService nlpService)
    {
        try
        {
            _logger.LogInformation("Estimating relevance for Article: {articleId} and ResearchQuestion: {question}", 
                article.Id, researchQuestion.Question);

            return await nlpService.EstimateRevelance(
            $"Title: {article.Title}\n Abstract: {article.Abstract} \n Metadata: {JsonSerializer.Serialize(article.KeySemantics)}",
            "abstract", researchQuestion.Question, [.. researchQuestion.CombinedDefinitions]);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to estimate relevance for Article: {articleId}", article.Id);
            throw new InvalidOperationException("Failed to estimate relevance for Article", ex);
        }
    }
}
