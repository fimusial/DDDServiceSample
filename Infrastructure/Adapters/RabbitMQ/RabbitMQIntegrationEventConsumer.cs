using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Infrastructure;

public sealed class RabbitMQIntegrationEventConsumer : IDisposable
{
    private readonly ILogger<RabbitMQIntegrationEventConsumer> logger;
    private readonly RabbitMQConfiguration configuration;
    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly IConnection connection;

    private IModel? channel;

    public RabbitMQIntegrationEventConsumer(
        ILogger<RabbitMQIntegrationEventConsumer> logger,
        IOptions<RabbitMQConfiguration> configuration,
        IServiceScopeFactory serviceScopeFactory,
        IConnection connection)
    {
        this.logger = logger;
        this.configuration = configuration.Value;
        this.serviceScopeFactory = serviceScopeFactory;
        this.connection = connection;
    }

    public void Subscribe()
    {
        logger.LogMethodRunning(nameof(Subscribe));

        if (channel is not null)
        {
            throw new InvalidOperationException($"{nameof(RabbitMQIntegrationEventConsumer)} is already subscribed");
        }

        channel = connection.CreateModel();

        var consumerName = nameof(RabbitMQIntegrationEventConsumer);
        var dlqName = $"dlq.{consumerName}";

        DeclareDeadLetterQueueAndExchange(dlqName);
        var consumerQueueDeclared = DeclareConsumerQueueAndExchange(consumerName);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.Received += OnReceivedAsync;

        channel.BasicConsume(queue: consumerQueueDeclared.QueueName, autoAck: false, consumer: consumer);
    }

    public void Dispose()
    {
        logger.LogMethodRunning(nameof(Dispose));

        channel?.Dispose();
    }

    private async Task OnReceivedAsync(object sender, BasicDeliverEventArgs eventArgs)
    {
#pragma warning disable CA1031 // Do not catch general exception types
        try
        {
            var message = Encoding.UTF8.GetString(eventArgs.Body.Span);
            var integrationEvent = IntegrationEvent.JsonDeserialize(message);

            await using (var serviceScope = serviceScopeFactory.CreateAsyncScope())
            using (var loggerScope = serviceScope.ServiceProvider.CreateOperationContextLoggerScope())
            {
                var mediator = serviceScope.ServiceProvider.GetRequiredService<IMediator>();
                var unitOfWork = serviceScope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                await unitOfWork.BeginTransactionAsync(CancellationToken.None);
                await mediator.Publish(integrationEvent.ToNotification(), CancellationToken.None);
                await unitOfWork.CommitTransactionAsync(CancellationToken.None);
            }

            channel!.BasicAck(deliveryTag: eventArgs.DeliveryTag, multiple: false);
        }
        catch (Exception)
        {
            channel!.BasicReject(deliveryTag: eventArgs.DeliveryTag, requeue: false);
        }
#pragma warning restore CA1031 // Do not catch general exception types
    }

    private QueueDeclareOk DeclareDeadLetterQueueAndExchange(string dlqName)
    {
        var exchange = configuration.IntegrationEventsDeadLetterExchangeName;

        channel!.ExchangeDeclare(
            exchange, ExchangeType.Direct, durable: true);

        var queueDeclared = channel!.QueueDeclare(
            dlqName, durable: true, exclusive: false, autoDelete: false, arguments: null);

        channel!.QueueBind(
            queueDeclared.QueueName, exchange: exchange, routingKey: "#");

        return queueDeclared;
    }

    private QueueDeclareOk DeclareConsumerQueueAndExchange(string consumerName)
    {
        var exchange = configuration.IntegrationEventsExchangeName;

        channel!.ExchangeDeclare(
            exchange, ExchangeType.Topic, durable: true);

        var args = new Dictionary<string, object>
        {
            { "x-dead-letter-exchange", configuration.IntegrationEventsDeadLetterExchangeName },
            { "x-dead-letter-routing-key", "#" },
        };

        var queueDeclared = channel!.QueueDeclare(
            consumerName, durable: true, exclusive: false, autoDelete: false, arguments: args);

        channel!.QueueBind(
            queueDeclared.QueueName, exchange: exchange, routingKey: "#");

        return queueDeclared;
    }
}