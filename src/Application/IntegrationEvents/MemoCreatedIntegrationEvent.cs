namespace Application;

public class MemoCreatedIntegrationEvent : IntegrationEvent
{
    public int MemoId { get; init; }
}