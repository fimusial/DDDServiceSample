using System.Collections.Generic;

namespace Application;

public interface IIntegrationEventPublisher
{
    void Publish(IEnumerable<IntegrationEvent> integrationEvents);
}