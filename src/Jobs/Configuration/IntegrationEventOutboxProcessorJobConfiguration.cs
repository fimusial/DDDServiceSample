namespace Jobs;

public class IntegrationEventOutboxProcessorJobConfiguration
{
    public int BatchSize { get; set; }

    required public string CronExpression { get; set; }

    public int StartDelaySeconds { get; set; }
}