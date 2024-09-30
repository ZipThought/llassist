using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

using llassist.Common;
using llassist.Common.Models;
using llassist.ApiService.Controllers;
using llassist.ApiService.Repositories;
using llassist.ApiService.Services;
using Microsoft.EntityFrameworkCore;
using llassist.ApiService.Repositories.Specifications;
using DotNetWorkQueue.Configuration;
using DotNetWorkQueue;
using DotNetWorkQueue.Transport.PostgreSQL.Basic;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;
using llassist.ApiService.Executors;
using DotNetWorkQueue.Queue;
using Microsoft.Extensions.Configuration;
using System;
using System.Text.Json;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add service defaults & Aspire components.
        builder.AddServiceDefaults();

        // Add services to the container.
        builder.Services.AddProblemDetails();

        // Register the persistence repository.
        var dbConnectionString = builder.Configuration.GetConnectionString("LlassistAppDatabase");
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(dbConnectionString);
        });
        builder.Services.AddScoped<ICRUDRepository<Ulid, Project, ProjectSearchSpec>, ProjectRepository>();
        builder.Services.AddScoped<ICRUDRepository<Ulid, Article, ArticleSearchSpec>, ArticleRepository>();
        builder.Services.AddScoped<ICRUDRepository<Ulid, EstimateRelevanceJob, EstimateRelevanceJobSearchSpec>, EstimateRelevanceJobRepository>();

        ConfigureQueue(builder.Services, builder.Configuration);

        // Register the Services
        builder.Services.AddScoped<ProjectService>();
        builder.Services.AddScoped<ArticleService>();
        builder.Services.AddSingleton<LLMService>(provider =>
        {
            var configuration = provider.GetRequiredService<IConfiguration>();
            var openAIAPIKey = configuration["OpenAI:ApiKey"];
            return new LLMService(openAIAPIKey);
        });
        builder.Services.AddScoped<INLPService, NLPService>();

        // Register the Controllers
        builder.Services.AddControllers();

        // Register Swagger services
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "LLAssist API", Version = "v1" });
        });

        var app = builder.Build();

        // Immediately initiate the queue consumer
        var backgroundTaskExecutor = app.Services.GetRequiredService<BackgroundTaskExecutor>();

        // Configure the HTTP request pipeline.
        app.UseExceptionHandler();

        // Enable routing
        app.UseRouting();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Companion API V1");
            });
        }

        // Enable endpoints for controllers
        app.MapControllers();
        app.MapDefaultEndpoints();

        app.Run();
    }

    private static void ConfigureQueue(IServiceCollection services, IConfiguration configuration)
    {
        // Create the queue if it doesn't exist
        var queueConfiguration = new QueueConfiguration();
        configuration.GetSection(QueueConfiguration.SectionName).Bind(queueConfiguration);
        services.AddSingleton(queueConfiguration);

        var queueName = queueConfiguration.QueueName;
        var dbConnectionString = configuration.GetConnectionString("LlassistAppDatabase");
        var queueConnection = new QueueConnection(queueName, dbConnectionString);

        // Register queue container, create the queue if it doesn't exist
        services.AddSingleton(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<Program>>();

            using (var createQueueContainer = new QueueCreationContainer<PostgreSqlMessageQueueInit>())
            {
                using var createQueue = createQueueContainer.GetQueueCreation<PostgreSqlMessageQueueCreation>(queueConnection);
                if (!createQueue.QueueExists)
                {
                    logger.LogInformation("Queue {QueueName} does not exist. Creating queue...", queueName);

                    createQueue.Options.EnableDelayedProcessing = queueConfiguration.EnableDelayedProcessing;
                    createQueue.Options.EnableHeartBeat = queueConfiguration.EnableHeartBeat;
                    createQueue.Options.EnableMessageExpiration = queueConfiguration.EnableMessageExpiration;
                    createQueue.Options.EnableStatus = queueConfiguration.EnableStatus;

                    logger.LogInformation("Queue options set: EnableDelayedProcessing={EnableDelayedProcessing}, EnableHeartBeat={EnableHeartBeat}, EnableMessageExpiration={EnableMessageExpiration}, EnableStatus={EnableStatus}",
                        createQueue.Options.EnableDelayedProcessing,
                        createQueue.Options.EnableHeartBeat,
                        createQueue.Options.EnableMessageExpiration,
                        createQueue.Options.EnableStatus);

                    createQueue.CreateQueue();
                    logger.LogInformation("Queue {QueueName} created successfully", queueName);
                }
            }

            var queueContainer = new QueueContainer<PostgreSqlMessageQueueInit>();
            return queueContainer;
        });

        // Register queue producer
        services.AddSingleton(sp =>
        {
            var queueContainer = sp.GetRequiredService<QueueContainer<PostgreSqlMessageQueueInit>>();
            return queueContainer.CreateProducer<BackgroundTask>(queueConnection);
        });
        services.AddScoped<EstimateRelevanceService>();

        // Register queue consumer and executor
        services.AddSingleton(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<Program>>();

            var queueContainer = sp.GetRequiredService<QueueContainer<PostgreSqlMessageQueueInit>>();
            var consumerQueue = queueContainer.CreateConsumer(queueConnection);
            ConfigureConsumerQueue(consumerQueue, queueConfiguration, logger);
            return consumerQueue;
        });
        services.AddSingleton<BackgroundTaskExecutor>();
    }

    private static void ConfigureConsumerQueue(IConsumerQueue consumerQueue, QueueConfiguration queueConfiguration, ILogger<Program> logger)
    {
        logger.LogInformation("Configuring ConsumerQueue with: {config}", JsonSerializer.Serialize(queueConfiguration));

        consumerQueue.Configuration.HeartBeat.UpdateTime = queueConfiguration.HeartBeatUpdateTime;
        consumerQueue.Configuration.Worker.WorkerCount = queueConfiguration.ConsumerWorkerCount;
        consumerQueue.Configuration.HeartBeat.UpdateTime = queueConfiguration.HeartBeatUpdateTime;
        consumerQueue.Configuration.HeartBeat.MonitorTime = TimeSpan.FromSeconds(queueConfiguration.HeartBeatMonitorTimeInSec);
        consumerQueue.Configuration.HeartBeat.Time = TimeSpan.FromSeconds(queueConfiguration.HeartBeatTimeInSec);
        consumerQueue.Configuration.MessageExpiration.Enabled = queueConfiguration.EnableMessageExpiration;
        consumerQueue.Configuration.MessageExpiration.MonitorTime = TimeSpan.FromSeconds(queueConfiguration.MessageExpirationMonitorTimeInSec);
        consumerQueue.Configuration.TransportConfiguration.RetryDelayBehavior.Add(typeof(InvalidDataException),
            [TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(6), TimeSpan.FromSeconds(9)]); // TODO move hardcoded values to config
    }
}
