using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Application;

public class PublishIntegrationEventsCommandHandler : IRequestHandler<PublishIntegrationEventsCommand, Unit>
{
    private readonly IIntegrationEventOutbox outbox;

    public PublishIntegrationEventsCommandHandler(IIntegrationEventOutbox outbox)
    {
        this.outbox = outbox;
    }

    public async Task<Unit> Handle(PublishIntegrationEventsCommand request, CancellationToken cancellationToken)
    {
        var eventsToPublish = await outbox.PopBatchAsync(request.BatchSize, cancellationToken);

        foreach (var eventToPublish in eventsToPublish)
        {
            // TODO: publish events to a message broker (at-least-once delivery)
            System.Console.WriteLine($"publishing {eventToPublish.Type}:{eventToPublish.Id}");
        }

        return Unit.Value;
    }
}