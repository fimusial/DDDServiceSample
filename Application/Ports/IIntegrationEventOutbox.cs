using System.Threading;
using System.Threading.Tasks;

namespace Application;

public interface IIntegrationEventOutbox
{
    Task PushAsync(IntegrationEvent integrationEvent, CancellationToken cancellationToken);
}