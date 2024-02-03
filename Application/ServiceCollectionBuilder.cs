using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ServiceCollectionBuilder
{
    public static IServiceCollection AddApplication(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddMediatR(config => config.RegisterServicesFromAssembly(typeof(ServiceCollectionBuilder).Assembly))
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(UnitOfWorkBehavior<,>));
    }
}