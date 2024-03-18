using System.Collections.Generic;

namespace Domain;

public abstract class Entity
{
    public int Id { get; set; }

    public IReadOnlyCollection<IDomainEvent> PublishedDomainEvents => DomainEvents.AsReadOnly();

    protected IList<IDomainEvent> DomainEvents { get; } = new List<IDomainEvent>();
}