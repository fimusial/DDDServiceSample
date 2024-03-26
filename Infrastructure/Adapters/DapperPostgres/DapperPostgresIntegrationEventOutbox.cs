using System.Collections.Generic;
using System.Linq;
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

    public Task EnqueueAsync(IntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        return npgsqlConnection.ExecuteAsync(new CommandDefinition(
            "INSERT INTO integration_event_outbox(content) VALUES(@Content)",
            parameters: new { Content = integrationEvent.JsonSerialize() },
            cancellationToken: cancellationToken));
    }

    public async Task<IEnumerable<IntegrationEvent>> DequeueBatchAsync(int batchSize, CancellationToken cancellationToken)
    {
        var batch = await npgsqlConnection.QueryAsync(new CommandDefinition(
            "SELECT * FROM integration_event_outbox ORDER BY pushed_at LIMIT @batchSize",
            parameters: new { batchSize },
            cancellationToken: cancellationToken));

        await npgsqlConnection.ExecuteAsync(new CommandDefinition(
            "DELETE FROM integration_event_outbox WHERE id = ANY(@BatchIds)",
            parameters: new { BatchIds = batch.Select(x => (int)x.id).ToArray() },
            cancellationToken: cancellationToken));

        return batch.Select(x => (IntegrationEvent)IntegrationEvent.JsonDeserialize(x.content)).ToList();
    }
}