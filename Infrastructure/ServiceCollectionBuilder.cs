using Application;
using Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Infrastructure;

public static class ServiceCollectionBuilder
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddOptions<PostgresConfiguration>().BindConfiguration(nameof(PostgresConfiguration));
        serviceCollection
            .AddScoped(serviceProvider => new Npgsql.NpgsqlConnection(serviceProvider.GetRequiredService<IOptions<PostgresConfiguration>>().Value.ConnectionString))
            .AddScoped<IUnitOfWork, DapperPostgresUnitOfWork>()
            .AddScoped<IRepository<Memo>, DapperPostgresMemoRepository>()
            .AddScoped<IIntegrationEventOutbox, DapperPostgresIntegrationEventOutbox>();

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
                    Password = configuration.Password
                };
            })
            .AddScoped<IMessageBroker, RabbitMQMessageBroker>();

        return serviceCollection;
    }
}