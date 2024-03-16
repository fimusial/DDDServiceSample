﻿using Application;
using Infrastructure;
using Jobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Quartz;

var builder = Host.CreateDefaultBuilder()
    .ConfigureAppConfiguration(builder =>
    {
        builder.AddJsonFile("appsettings.json");
    })
    .ConfigureServices((hostBuilderContext, serviceCollection) =>
    {
        serviceCollection
            .AddApplication()
            .AddInfrastructure()
            .AddJobs();

        serviceCollection.AddQuartz();
        serviceCollection.AddQuartzHostedService(opt =>
        {
            opt.WaitForJobsToComplete = true;
        });
    })
    .Build();

var scheduler = await builder.Services.GetRequiredService<ISchedulerFactory>().GetScheduler();

await IntegrationEventOutboxProcessorJob.ScheduleSelf(
    scheduler,
    builder.Services.GetRequiredService<IOptions<IntegrationEventOutboxProcessorJobConfiguration>>().Value);

await builder.RunAsync();