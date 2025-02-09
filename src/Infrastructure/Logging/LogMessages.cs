using System;
using Microsoft.Extensions.Logging;

namespace Infrastructure;

public static partial class LogMessages
{
    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "UnitOfWork step performed: {step}, transactionId: {transactionId}")]
    public static partial void LogUnitOfWorkStep(
        this ILogger logger,
        string step,
        string? transactionId);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "RabbitMQIntegrationEventConsumer step performed: {step}, messageId: {messageId}")]
    public static partial void LogRabbitMQIntegrationEventConsumerStep(
        this ILogger logger,
        string step,
        string messageId);
}