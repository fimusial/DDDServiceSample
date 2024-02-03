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

    public Task PushAsync(IntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        return npgsqlConnection.ExecuteAsync(new CommandDefinition(
            "INSERT INTO IntegrationEventOutbox(content, type) VALUES(@Content, @Type)",
            parameters: new
                {
                    Content = integrationEvent.JsonSerialize(),
                    integrationEvent.Type
                },
            cancellationToken: cancellationToken
            ));
    }

    public async Task<IEnumerable<IntegrationEvent>> PopBatchAsync(int batchSize, CancellationToken cancellationToken)
    {
        var batch = await npgsqlConnection.QueryAsync(new CommandDefinition(
            "SELECT * FROM IntegrationEventOutbox ORDER BY pushedAt LIMIT @batchSize",
            parameters: new { batchSize },
            cancellationToken: cancellationToken
        ));

        var integrationEvents = batch.Select(x => (IntegrationEvent)IntegrationEvent.JsonDeserialize(x.content, x.type)).ToList();

        await npgsqlConnection.ExecuteAsync(new CommandDefinition(
            "DELETE FROM IntegrationEventOutbox WHERE id = ANY(@BatchIds)",
            parameters: new{ BatchIds = batch.Select(x => (int)x.id).ToArray() },
            cancellationToken: cancellationToken
            ));

        return integrationEvents;
    }
}