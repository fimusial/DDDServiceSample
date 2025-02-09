using System;
using System.IO;
using System.Text.Json;
using Application;
using Infrastructure;
using Jobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
        configurationBuilder.AddJsonFile(Path.Combine(AppContext.BaseDirectory, "appsettings.json"));
        configurationBuilder.AddEnvironmentVariables();
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

await builder.Services.GetRequiredService<IntegrationEventOutboxProcessorJob>().ScheduleSelfAsync();

await builder.RunAsync();