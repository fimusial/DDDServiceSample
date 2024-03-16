using System.Threading;
using System.Threading.Tasks;
using Application;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Quartz;

namespace Jobs;

[DisallowConcurrentExecution]
public class IntegrationEventOutboxProcessorJob : IJob
{
    private readonly IServiceScopeFactory serviceScopeFactory;

    public IntegrationEventOutboxProcessorJob(IServiceScopeFactory serviceScopeFactory)
    {
        this.serviceScopeFactory = serviceScopeFactory;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        await using (var scope = serviceScopeFactory.CreateAsyncScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var configuration = scope.ServiceProvider.GetRequiredService<IOptions<IntegrationEventOutboxProcessorJobConfiguration>>().Value;

            await mediator.Send(new PublishIntegrationEventsCommand { BatchSize = configuration.BatchSize }, CancellationToken.None);
        }
    }

    public static async Task ScheduleSelf(
        IScheduler scheduler,
        IntegrationEventOutboxProcessorJobConfiguration configuration)
    {
        var job = JobBuilder.Create<IntegrationEventOutboxProcessorJob>()
            .WithIdentity(nameof(IntegrationEventOutboxProcessorJob))
            .Build();

        var trigger = TriggerBuilder.Create()
            .WithIdentity($"{nameof(IntegrationEventOutboxProcessorJob)}Trigger")
            .WithCronSchedule(configuration.CronExpression)
            .ForJob(job)
            .Build();

        await scheduler.ScheduleJob(job, trigger);
    }
}