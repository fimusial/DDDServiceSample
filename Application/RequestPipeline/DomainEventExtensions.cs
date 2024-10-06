using System;
using Domain;
using MediatR;

namespace Application;

public static class EventExtensions
{
    public static INotification ToNotification(this IDomainEvent domainEvent)
    {
        INotification? notification = null;

#pragma warning disable CA1031 // Do not catch general exception types
        try
        {
            var notificationGenericType = typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType());
            notification = (INotification)Activator.CreateInstance(notificationGenericType, domainEvent)!;
        }
        catch (Exception exception)
        {
            ThrowCouldNotInstantiateNotification(domainEvent, exception);
        }
#pragma warning restore CA1031 // Do not catch general exception types

        if (notification is null)
        {
            ThrowCouldNotInstantiateNotification(domainEvent);
        }

        return notification!;
    }

    public static INotification ToNotification(this IntegrationEvent integrationEvent)
    {
        INotification? notification = null;

#pragma warning disable CA1031 // Do not catch general exception types
        try
        {
            var notificationGenericType = typeof(IntegrationEventNotification<>).MakeGenericType(integrationEvent.GetType());
            notification = (INotification)Activator.CreateInstance(notificationGenericType, integrationEvent)!;
        }
        catch (Exception exception)
        {
            ThrowCouldNotInstantiateNotification(integrationEvent, exception);
        }
#pragma warning restore CA1031 // Do not catch general exception types

        if (notification is null)
        {
            ThrowCouldNotInstantiateNotification(integrationEvent);
        }

        return notification!;
    }

    private static void ThrowCouldNotInstantiateNotification(object? @event, Exception? exception = null)
    {
        var receivedEventTypeName = @event?.GetType().FullName ?? "null";

        throw new InvalidOperationException(
            $"could not instantiate notification for event: {receivedEventTypeName}",
            exception);
    }
}