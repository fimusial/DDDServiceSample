using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application;

public interface IIntegrationEventOutbox
{
    Task EnqueueAsync(IntegrationEvent integrationEvent, CancellationToken cancellationToken);

    Task<IEnumerable<IntegrationEvent>> DequeueBatchAsync(int batchSize, CancellationToken cancellationToken);
}