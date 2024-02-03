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
            .AddScoped<IRepository<Memo>, DapperPostgresMemoRepository>()
            .AddScoped<IIntegrationEventOutbox, DapperPostgresIntegrationEventOutbox>();

        return serviceCollection;
    }
}