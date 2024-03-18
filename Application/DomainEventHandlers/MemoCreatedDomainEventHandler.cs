using System.Threading;
using System.Threading.Tasks;
using Domain;
using MediatR;

namespace Application;

public class MemoCreatedDomainEventHandler : INotificationHandler<DomainEventNotification<MemoCreatedDomainEvent>>
{
    private readonly IIntegrationEventOutbox outbox;

    public MemoCreatedDomainEventHandler(IIntegrationEventOutbox outbox)
    {
        this.outbox = outbox;
    }

    public async Task Handle(
        DomainEventNotification<MemoCreatedDomainEvent> notification,
        CancellationToken cancellationToken)
    {
        await outbox.EnqueueAsync(
            new MemoCreatedIntegrationEvent()
            {
                MemoId = notification.DomainEvent.MemoId,
            },
            cancellationToken);
    }
}