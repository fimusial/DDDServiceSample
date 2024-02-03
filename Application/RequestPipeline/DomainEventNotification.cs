using Domain;
using MediatR;

namespace Application;

public record DomainEventNotification<T>(T DomainEvent) : INotification
    where T : IDomainEvent
{
}