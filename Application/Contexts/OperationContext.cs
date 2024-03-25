using System;

namespace Application;

public class OperationContext : IOperationContext
{
    private Guid? correlationId = null!;

    public Guid OperationId { get; } = Guid.NewGuid();

    public DateTime OperationUtcTimestamp { get; } = DateTime.UtcNow;

    public Guid CorrelationId
    {
        get
        {
            if (!correlationId.HasValue)
            {
                correlationId = Guid.NewGuid();
            }

            return correlationId.Value;
        }

        set
        {
            if (correlationId.HasValue)
            {
                throw new InvalidOperationException(
                    $"{nameof(CorrelationId)} has already been set for this instance of {nameof(OperationContext)}");
            }

            correlationId = value;
        }
    }
}