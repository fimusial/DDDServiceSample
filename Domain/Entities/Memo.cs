using System;

namespace Domain;

public class Memo : Entity
{
    public required string Content { get; set; }

    public void MemoCreated(int id)
    {
        if (Id > 0)
        {
            throw new InvalidOperationException("cannot assign an identifier, memo already has one");
        }

        if (id < 1)
        {
            throw new InvalidOperationException("attempted to assign an invalid identifier");
        }

        Id = id;
        domainEvents.Add(new MemoCreatedDomainEvent(Id));
    }
}