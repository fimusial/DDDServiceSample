using System;
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
        foreach (var @event in entity.DomainEvents)
        {
            object? notification = null;
            try
            {
                var notificationGenericType = typeof(DomainEventNotification<>).MakeGenericType(@event.GetType());
                notification = Activator.CreateInstance(notificationGenericType, @event);
            }
            catch(Exception) {}

            if (notification is null)
            {
                var receivedEventTypeName = @event?.GetType()?.FullName ?? "null";
                throw new InvalidOperationException($"could not instantiate notification for domain event: {receivedEventTypeName}");
            }

            await mediator.Publish(notification, cancellationToken);
        }
    }
}