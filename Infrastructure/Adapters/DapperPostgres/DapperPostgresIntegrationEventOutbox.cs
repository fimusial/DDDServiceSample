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
    private readonly IUnitOfWork unitOfWork;

    public DapperPostgresIntegrationEventOutbox(
        NpgsqlConnection npgsqlConnection,
        IUnitOfWork unitOfWork)
    {
        this.npgsqlConnection = npgsqlConnection;
        this.unitOfWork = unitOfWork;
    }

    public Task EnqueueAsync(IntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        unitOfWork.ThrowIfNoOngoingTransaction();

        return npgsqlConnection.ExecuteAsync(new CommandDefinition(
            "INSERT INTO IntegrationEventOutbox(content) VALUES(@Content)",
            parameters: new
            {
                Content = integrationEvent.JsonSerialize(),
            },
            cancellationToken: cancellationToken
            ));
    }

    public async Task<IEnumerable<IntegrationEvent>> DequeueBatchAsync(int batchSize, CancellationToken cancellationToken)
    {
        unitOfWork.ThrowIfNoOngoingTransaction();

        var batch = await npgsqlConnection.QueryAsync(new CommandDefinition(
            "SELECT * FROM IntegrationEventOutbox ORDER BY pushedAt LIMIT @batchSize",
            parameters: new { batchSize },
            cancellationToken: cancellationToken
        ));

        var integrationEvents = batch.Select(x => (IntegrationEvent)IntegrationEvent.JsonDeserialize(x.content)).ToList();

        await npgsqlConnection.ExecuteAsync(new CommandDefinition(
            "DELETE FROM IntegrationEventOutbox WHERE id = ANY(@BatchIds)",
            parameters: new { BatchIds = batch.Select(x => (int)x.id).ToArray() },
            cancellationToken: cancellationToken
            ));

        return integrationEvents;
    }
}