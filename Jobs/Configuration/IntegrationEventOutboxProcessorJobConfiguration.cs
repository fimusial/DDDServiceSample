public class IntegrationEventOutboxProcessorJobConfiguration
{
    public int BatchSize { get; set; }
    public required string CronExpression { get; set; }
}