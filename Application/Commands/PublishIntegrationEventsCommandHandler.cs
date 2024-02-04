using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Application;

public class PublishIntegrationEventsCommandHandler : IRequestHandler<PublishIntegrationEventsCommand, Unit>
{
    private readonly IIntegrationEventOutbox outbox;
    private readonly IIntegrationEventPublisher publisher;

    public PublishIntegrationEventsCommandHandler(IIntegrationEventOutbox outbox, IIntegrationEventPublisher publisher)
    {
        this.outbox = outbox;
        this.publisher = publisher;
    }

    public async Task<Unit> Handle(PublishIntegrationEventsCommand request, CancellationToken cancellationToken)
    {
        var eventsToPublish = await outbox.DequeueBatchAsync(request.BatchSize, cancellationToken);
        publisher.Publish(eventsToPublish);
        return Unit.Value;
    }
}