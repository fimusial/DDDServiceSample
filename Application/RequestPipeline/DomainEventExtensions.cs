using System;
using Domain;
using MediatR;

namespace Application;

public static class DomainEventExtensions
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
        catch (Exception)
        {
        }
#pragma warning restore CA1031 // Do not catch general exception types

        if (notification is null)
        {
            var receivedEventTypeName = domainEvent?.GetType()?.FullName ?? "null";
            throw new InvalidOperationException($"could not instantiate notification for domain event: {receivedEventTypeName}");
        }

        return notification;
    }
}