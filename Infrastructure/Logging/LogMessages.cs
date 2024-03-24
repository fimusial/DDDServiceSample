using Microsoft.Extensions.Logging;

namespace Infrastructure;

public static partial class LogMessages
{
    [LoggerMessage(Level = LogLevel.Information, Message = "UnitOfWork step performed {step} {transactionId}")]
    public static partial void LogUnitOfWorkStep(this ILogger logger, string step, string? transactionId);
}