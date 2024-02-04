using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Application;

public class MemoCreatedIntegrationEventHandler : INotificationHandler<IntegrationEventNotification<MemoCreatedIntegrationEvent>>
{
    private readonly IMediator mediator;

    public MemoCreatedIntegrationEventHandler(IMediator mediator)
    {
        this.mediator = mediator;
    }

    public async Task Handle(
        IntegrationEventNotification<MemoCreatedIntegrationEvent> notification,
        CancellationToken cancellationToken)
    {
        await mediator.Send(new AddMemoCommand { Content = "loop it!" }, cancellationToken);
    }
}