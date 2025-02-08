using Microsoft.Extensions.DependencyInjection;

namespace WebAPI;

public static class ServiceCollectionBuilder
{
    public static IServiceCollection AddWebAPI(this IServiceCollection serviceCollection)
    {
        return serviceCollection;
    }
}