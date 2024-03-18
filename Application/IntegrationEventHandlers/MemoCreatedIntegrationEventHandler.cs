using System;
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

    #pragma warning disable CA5394
    #pragma warning disable CA2201
    public async Task Handle(
        IntegrationEventNotification<MemoCreatedIntegrationEvent> notification,
        CancellationToken cancellationToken)
    {
        if (new Random().Next(0, 2) == 1)
        {
            throw new Exception("unexpected failure!");
        }

        await mediator.Send(new AddMemoCommand { Content = "loop it!" }, cancellationToken);
    }
}