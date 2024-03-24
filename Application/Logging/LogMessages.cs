using Microsoft.Extensions.Logging;

namespace Application;

public static partial class LogMessages
{
    [LoggerMessage(Level = LogLevel.Information, Message = "Handler running {handler}")]
    public static partial void LogHandlerRunning(this ILogger logger, string handler);

    [LoggerMessage(Level = LogLevel.Information, Message = "Method running {method}")]
    public static partial void LogMethodRunning(this ILogger logger, string method);
}