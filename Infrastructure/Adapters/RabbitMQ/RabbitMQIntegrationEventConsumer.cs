using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Infrastructure;

public class RabbitMQIntegrationEventConsumer : IDisposable
{
    private readonly RabbitMQConfiguration configuration;
    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly IConnection connection;

    private IModel? channel = null;

    public RabbitMQIntegrationEventConsumer(
        IOptions<RabbitMQConfiguration> configuration,
        IServiceScopeFactory serviceScopeFactory,
        IConnection connection)
    {
        Console.WriteLine("RabbitMQIntegrationEventConsumer:Ctor");

        this.configuration = configuration.Value;
        this.serviceScopeFactory = serviceScopeFactory;
        this.connection = connection;
    }

    private async Task OnReceivedAsync(object sender, BasicDeliverEventArgs eventArgs)
    {
        try
        {
            var message = Encoding.UTF8.GetString(eventArgs.Body.Span);
            var integrationEvent = IntegrationEvent.JsonDeserialize(message);

            await using (var scope = serviceScopeFactory.CreateAsyncScope())
            {
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

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
            { "x-dead-letter-routing-key", "#" }
        };

        var queueDeclared = channel!.QueueDeclare(
            consumerName, durable: true, exclusive: false, autoDelete: false, arguments: args);

        channel!.QueueBind(
            queueDeclared.QueueName, exchange: exchange, routingKey: "#");

        return queueDeclared;
    }

    public void Subscribe()
    {
        Console.WriteLine("RabbitMQIntegrationEventConsumer:Subscribe");

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
        Console.WriteLine("RabbitMQIntegrationEventConsumer:Dispose");

        channel?.Dispose();
    }
}