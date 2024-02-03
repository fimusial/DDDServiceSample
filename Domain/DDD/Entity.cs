using System.Collections.Generic;

namespace Domain;

public abstract class Entity
{
    public int Id { get; set; }

    protected List<IDomainEvent> domainEvents = new List<IDomainEvent>();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => domainEvents.AsReadOnly();
}