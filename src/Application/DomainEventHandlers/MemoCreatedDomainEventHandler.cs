using System.Threading;
using System.Threading.Tasks;
using Domain;
using MediatR;

namespace Application;

public class MemoCreatedDomainEventHandler : INotificationHandler<DomainEventNotification<MemoCreatedDomainEvent>>
{
    private readonly IIntegrationEventOutbox outbox;
    private readonly IOperationContext operationContext;

    public MemoCreatedDomainEventHandler(
        IIntegrationEventOutbox outbox,
        IOperationContext operationContext)
    {
        this.outbox = outbox;
        this.operationContext = operationContext;
    }

    public async Task Handle(
        DomainEventNotification<MemoCreatedDomainEvent> notification,
        CancellationToken cancellationToken)
    {
        await outbox.EnqueueAsync(
            new MemoCreatedIntegrationEvent()
            {
                MemoId = notification.DomainEvent.MemoId,
                CorrelationId = operationContext.CorrelationId,
            },
            cancellationToken);
    }
}