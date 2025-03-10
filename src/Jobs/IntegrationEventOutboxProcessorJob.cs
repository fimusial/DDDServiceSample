using System;
using System.Threading;
using System.Threading.Tasks;
using Application;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;

namespace Jobs;

[DisallowConcurrentExecution]
public class IntegrationEventOutboxProcessorJob : IJob
{
    private readonly ILogger<IntegrationEventOutboxProcessorJob> logger;
    private readonly IntegrationEventOutboxProcessorJobConfiguration configuration;
    private readonly ISchedulerFactory schedulerFactory;
    private readonly IServiceScopeFactory serviceScopeFactory;

    public IntegrationEventOutboxProcessorJob(
        ILogger<IntegrationEventOutboxProcessorJob> logger,
        IOptions<IntegrationEventOutboxProcessorJobConfiguration> configuration,
        ISchedulerFactory schedulerFactory,
        IServiceScopeFactory serviceScopeFactory)
    {
        this.logger = logger;
        this.configuration = configuration.Value;
        this.schedulerFactory = schedulerFactory;
        this.serviceScopeFactory = serviceScopeFactory;
    }

    public async Task ScheduleSelfAsync()
    {
        var scheduler = await schedulerFactory.GetScheduler();

        var job = JobBuilder.Create<IntegrationEventOutboxProcessorJob>()
            .WithIdentity(nameof(IntegrationEventOutboxProcessorJob))
            .Build();

        var trigger = TriggerBuilder.Create()
            .WithIdentity($"{nameof(IntegrationEventOutboxProcessorJob)}Trigger")
            .WithCronSchedule(configuration.CronExpression)
            .StartAt(DateTimeOffset.UtcNow.AddSeconds(configuration.StartDelaySeconds))
            .ForJob(job)
            .Build();

        await scheduler.ScheduleJob(job, trigger);
    }

    public async Task Execute(IJobExecutionContext context)
    {
        IDisposable? loggerScope = null;

        try
        {
            await using (var serviceScope = serviceScopeFactory.CreateAsyncScope())
            {
                loggerScope = serviceScope.ServiceProvider.CreateOperationContextLoggerScope();

                var mediator = serviceScope.ServiceProvider.GetRequiredService<IMediator>();
                var configuration = serviceScope.ServiceProvider.GetRequiredService<IOptions<IntegrationEventOutboxProcessorJobConfiguration>>().Value;

                await mediator.Send(new PublishIntegrationEventsCommand { BatchSize = configuration.BatchSize }, CancellationToken.None);
            }
        }
        catch (Exception exception)
        {
            logger.LogException(exception);
            throw;
        }
        finally
        {
            loggerScope?.Dispose();
        }
    }
}