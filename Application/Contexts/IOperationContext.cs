using System;

namespace Application;

public interface IOperationContext
{
    Guid OperationId { get; }

    DateTime OperationUtcTimestamp { get; }
}