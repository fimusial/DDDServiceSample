using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application;

public class PublishIntegrationEventsCommandHandler : IRequestHandler<PublishIntegrationEventsCommand, Unit>
{
    private readonly ILogger<PublishIntegrationEventsCommandHandler> logger;
    private readonly IIntegrationEventOutbox outbox;
    private readonly IIntegrationEventPublisher publisher;

    public PublishIntegrationEventsCommandHandler(
        ILogger<PublishIntegrationEventsCommandHandler> logger,
        IIntegrationEventOutbox outbox,
        IIntegrationEventPublisher publisher)
    {
        this.logger = logger;
        this.outbox = outbox;
        this.publisher = publisher;
    }

    public async Task<Unit> Handle(PublishIntegrationEventsCommand request, CancellationToken cancellationToken)
    {
        logger.LogHandlerRunning(nameof(PublishIntegrationEventsCommandHandler));

        var eventsToPublish = await outbox.DequeueBatchAsync(request.BatchSize, cancellationToken);
        publisher.PublishBatch(eventsToPublish);
        return Unit.Value;
    }
}