using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using MediatR;

namespace Application;

public static class DomainEventNotificationDispatcher
{
    public static async Task DispatchDomainEventsAsync(this IMediator mediator, IEnumerable<Entity> entities, CancellationToken cancellationToken)
    {
        foreach (var entity in entities)
        {
            await mediator.DispatchDomainEventsAsync(entity, cancellationToken);
        }
    }

    public static async Task DispatchDomainEventsAsync(this IMediator mediator, Entity entity, CancellationToken cancellationToken)
    {
        foreach (var @event in entity.PublishedDomainEvents)
        {
            await mediator.Publish(@event.ToNotification(), cancellationToken);
        }
    }
}