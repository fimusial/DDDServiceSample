using System;

namespace Application;

public class OperationContext : IOperationContext
{
    public Guid OperationId { get; } = Guid.NewGuid();

    public DateTime OperationUtcTimestamp { get; } = DateTime.UtcNow;
}