using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Application;

public class PublishIntegrationEventsCommandHandler : IRequestHandler<PublishIntegrationEventsCommand, Unit>
{
    private readonly IIntegrationEventOutbox outbox;
    private readonly IMessageBroker messageBroker;

    public PublishIntegrationEventsCommandHandler(IIntegrationEventOutbox outbox, IMessageBroker messageBroker)
    {
        this.outbox = outbox;
        this.messageBroker = messageBroker;
    }

    public async Task<Unit> Handle(PublishIntegrationEventsCommand request, CancellationToken cancellationToken)
    {
        var eventsToPublish = await outbox.DequeueBatchAsync(request.BatchSize, cancellationToken);
        messageBroker.Publish(eventsToPublish);
        return Unit.Value;
    }
}