using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ServiceCollectionBuilder
{
    public static IServiceCollection AddApplication(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddMediatR(config => config.RegisterServicesFromAssembly(typeof(ServiceCollectionBuilder).Assembly))
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(UnitOfWorkBehavior<,>))
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(DisallowHandlerNestingBehavior<,>))
            .AddScoped<IOperationContext, OperationContext>();

        return serviceCollection;
    }
}