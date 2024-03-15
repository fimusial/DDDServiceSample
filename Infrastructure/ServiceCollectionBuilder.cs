using Application;
using Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Infrastructure;

public static class ServiceCollectionBuilder
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddOptions<PostgresConfiguration>().BindConfiguration(nameof(PostgresConfiguration));
        serviceCollection
            .AddScoped(serviceProvider => new NpgsqlConnection(serviceProvider.GetRequiredService<IOptions<PostgresConfiguration>>().Value.ConnectionString))
            .AddScoped<IUnitOfWork, DapperPostgresUnitOfWork>()
            .AddScoped<IRepository<Memo>>(serviceProvider => UnitOfWorkSafeguard<IRepository<Memo>>.CreateProxy(new DapperPostgresMemoRepository(serviceProvider.GetRequiredService<NpgsqlConnection>()), serviceProvider.GetRequiredService<IUnitOfWork>()))
            .AddScoped<IIntegrationEventOutbox>(serviceProvider => UnitOfWorkSafeguard<IIntegrationEventOutbox>.CreateProxy(new DapperPostgresIntegrationEventOutbox(serviceProvider.GetRequiredService<NpgsqlConnection>()), serviceProvider.GetRequiredService<IUnitOfWork>()));

        serviceCollection.AddOptions<RabbitMQConfiguration>().BindConfiguration(nameof(RabbitMQConfiguration));
        serviceCollection
            .AddSingleton(serviceProvider =>
            {
                var configuration = serviceProvider.GetRequiredService<IOptions<RabbitMQConfiguration>>().Value;
                return new RabbitMQ.Client.ConnectionFactory
                {
                    HostName = configuration.HostName,
                    Port = configuration.Port,
                    UserName = configuration.Username,
                    Password = configuration.Password,
                    DispatchConsumersAsync = true
                };
            })
            .AddScoped<IIntegrationEventPublisher, RabbitMQIntegrationEventPublisher>()
            .AddSingleton<RabbitMQIntegrationEventConsumer>();

        return serviceCollection;
    }
}