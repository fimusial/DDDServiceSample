namespace Application;

public class MemoCreatedIntegrationEvent : IntegrationEvent
{
    public override string Type => nameof(MemoCreatedIntegrationEvent);
    public override uint Version => 1;
}