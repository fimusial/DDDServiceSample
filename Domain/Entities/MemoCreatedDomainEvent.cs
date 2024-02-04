namespace Domain;

public record MemoCreatedDomainEvent(int MemoId) : IDomainEvent
{
}