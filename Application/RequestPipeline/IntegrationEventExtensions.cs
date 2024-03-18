using System;
using MediatR;

namespace Application;

public static class IntegrationEventExtensions
{
    public static INotification ToNotification(this IntegrationEvent integrationEvent)
    {
        INotification? notification = null;
#pragma warning disable CA1031 // Do not catch general exception types
        try
        {
            var notificationGenericType = typeof(IntegrationEventNotification<>).MakeGenericType(integrationEvent.GetType());
            notification = (INotification)Activator.CreateInstance(notificationGenericType, integrationEvent)!;
        }
        catch (Exception)
        {
        }
#pragma warning restore CA1031 // Do not catch general exception types

        if (notification is null)
        {
            var receivedEventTypeName = integrationEvent?.GetType()?.FullName ?? "null";
            throw new InvalidOperationException($"could not instantiate notification for integration event: {receivedEventTypeName}");
        }

        return notification;
    }
}