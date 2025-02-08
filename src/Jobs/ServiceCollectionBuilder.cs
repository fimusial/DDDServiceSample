using Microsoft.Extensions.DependencyInjection;

namespace Jobs;

public static class ServiceCollectionBuilder
{
    public static IServiceCollection AddJobs(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddOptions<IntegrationEventOutboxProcessorJobConfiguration>().BindConfiguration(nameof(IntegrationEventOutboxProcessorJobConfiguration));
        serviceCollection.AddSingleton<IntegrationEventOutboxProcessorJob>();

        return serviceCollection;
    }
}