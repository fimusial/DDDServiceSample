using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Application;

public static class LoggingServiceProviderExtensions
{
    public static IDisposable CreateOperationContextLoggerScope(this IServiceProvider serviceProvider)
    {
        var operationContext = serviceProvider.GetRequiredService<IOperationContext>();
        var logger = serviceProvider.GetRequiredService<ILogger<IOperationContext>>();

        var logProperties = new Dictionary<string, object>
        {
            { nameof(operationContext.OperationId), operationContext.OperationId },
            { nameof(operationContext.OperationUtcTimestamp), operationContext.OperationUtcTimestamp },
        };

        return logger.BeginScope(logProperties) ?? throw new InvalidOperationException("could not create logger scope");
    }
}