using System;
using MediatR;

namespace Application;

public static class IntegrationEventExtensions
{
    public static INotification ToNotification(this IntegrationEvent integrationEvent)
    {
        INotification? notification = null;
        try
        {
            var notificationGenericType = typeof(IntegrationEventNotification<>).MakeGenericType(integrationEvent.GetType());
            notification = (INotification)Activator.CreateInstance(notificationGenericType, integrationEvent)!;
        }
        catch(Exception) {}

        if (notification is null)
        {
            var receivedEventTypeName = integrationEvent?.GetType()?.FullName ?? "null";
            throw new InvalidOperationException($"could not instantiate notification for integration event: {receivedEventTypeName}");
        }

        return notification;
    }
}