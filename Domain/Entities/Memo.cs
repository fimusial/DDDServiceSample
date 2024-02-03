namespace Domain;

public class Memo : Entity
{
    public required string Content { get; set; }

    public Memo()
    {
        domainEvents.Add(new MemoCreatedDomainEvent());
    }
}