using System.Text.Json;
using Application;
using Infrastructure;
using Jobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;

var builder = Host.CreateDefaultBuilder()
    .ConfigureLogging(loggingBuilder =>
    {
        loggingBuilder.AddJsonConsole(formatterOptions =>
        {
            formatterOptions.IncludeScopes = true;
            formatterOptions.UseUtcTimestamp = true;
            formatterOptions.JsonWriterOptions = new JsonWriterOptions { Indented = true };
        });
    })
    .ConfigureAppConfiguration(configurationBuilder =>
    {
        configurationBuilder.AddJsonFile("appsettings.json");
    })
    .ConfigureServices((_, serviceCollection) =>
    {
        serviceCollection
            .AddApplication()
            .AddRepositoryInfrastructure()
            .AddIntegrationEventsInfrastructure()
            .AddJobs();

        serviceCollection.AddQuartz();
        serviceCollection.AddQuartzHostedService(opt =>
        {
            opt.WaitForJobsToComplete = true;
        });
    })
    .Build();

await IntegrationEventOutboxProcessorJob.ScheduleSelfAsync(
    await builder.Services.GetRequiredService<ISchedulerFactory>().GetScheduler(),
    builder.Services.GetRequiredService<IOptions<IntegrationEventOutboxProcessorJobConfiguration>>().Value);

builder.Services.GetRequiredService<RabbitMQIntegrationEventConsumer>().Subscribe();

await builder.RunAsync();