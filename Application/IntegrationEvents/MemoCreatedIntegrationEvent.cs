namespace Application;

public class MemoCreatedIntegrationEvent : IntegrationEvent
{
    public override string Type => nameof(MemoCreatedIntegrationEvent);

    public int MemoId { get; set; }
}