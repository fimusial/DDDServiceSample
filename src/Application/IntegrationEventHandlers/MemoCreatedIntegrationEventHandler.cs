using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application;

public class MemoCreatedIntegrationEventHandler : INotificationHandler<IntegrationEventNotification<MemoCreatedIntegrationEvent>>
{
    private readonly ILogger<MemoCreatedIntegrationEventHandler> logger;
    private readonly IMediator mediator;

    public MemoCreatedIntegrationEventHandler(
        ILogger<MemoCreatedIntegrationEventHandler> logger,
        IMediator mediator)
    {
        this.logger = logger;
        this.mediator = mediator;
    }

#pragma warning disable CA5394
#pragma warning disable CA2201
    public async Task Handle(
        IntegrationEventNotification<MemoCreatedIntegrationEvent> notification,
        CancellationToken cancellationToken)
    {
        logger.LogHandlerRunning(nameof(MemoCreatedIntegrationEventHandler));

        if (new Random().Next(0, 2) == 1)
        {
            throw new Exception("unexpected failure!");
        }

        await mediator.Send(new AddMemoCommand { Content = "loop it!" }, cancellationToken);
    }
}