using MediatR;

namespace Application;

public record IntegrationEventNotification<T>(T IntegrationEvent) : INotification
    where T : IntegrationEvent
{
}