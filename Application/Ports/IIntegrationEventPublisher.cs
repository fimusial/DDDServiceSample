using System.Collections.Generic;

namespace Application;

public interface IIntegrationEventPublisher
{
    void PublishBatch(IEnumerable<IntegrationEvent> integrationEvents);
}