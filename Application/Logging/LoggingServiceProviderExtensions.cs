using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Application;

public static class LoggingServiceProviderExtensions
{
    public static IDisposable CreateOperationContextLoggerScope(this IServiceProvider serviceProvider, Guid? continueWithCorrelationId = null)
    {
        var operationContext = serviceProvider.GetRequiredService<IOperationContext>();
        var logger = serviceProvider.GetRequiredService<ILogger<IOperationContext>>();

        if (continueWithCorrelationId.HasValue)
        {
            operationContext.CorrelationId = continueWithCorrelationId.Value;
        }

        var logProperties = new Dictionary<string, object>
        {
            { nameof(operationContext.OperationId), operationContext.OperationId },
            { nameof(operationContext.OperationUtcTimestamp), operationContext.OperationUtcTimestamp.ToString(ZuluDateTime.Format) },
            { nameof(operationContext.CorrelationId), operationContext.CorrelationId },
        };

        return logger.BeginScope(logProperties) ?? throw new InvalidOperationException("could not create logger scope");
    }
}