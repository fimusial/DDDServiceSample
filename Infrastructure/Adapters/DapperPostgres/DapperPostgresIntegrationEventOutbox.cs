using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Application;
using Dapper;
using Npgsql;

namespace Infrastructure;

public class DapperPostgresIntegrationEventOutbox : IIntegrationEventOutbox
{
    private readonly NpgsqlConnection npgsqlConnection;

    public DapperPostgresIntegrationEventOutbox(NpgsqlConnection npgsqlConnection)
    {
        this.npgsqlConnection = npgsqlConnection;
    }

    public Task PushAsync(IntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        var eventContent = JsonSerializer.Serialize(integrationEvent);

        return npgsqlConnection.ExecuteAsync(new CommandDefinition(
            "INSERT INTO IntegrationEventOutbox(eventContent) VALUES(@eventContent)",
            parameters: new { eventContent },
            cancellationToken: cancellationToken
            ));
    }
}