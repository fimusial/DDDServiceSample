using System.Collections.Generic;

namespace Application;

public interface IMessageBroker
{
    void Publish(IEnumerable<IntegrationEvent> integrationEvents);
}