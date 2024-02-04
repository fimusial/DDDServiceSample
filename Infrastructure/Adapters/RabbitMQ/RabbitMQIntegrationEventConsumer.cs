using System;
using System.Text;
using System.Threading;
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

    private IConnection persistentConnection;
    private IModel persistentChannel = null!;

    public RabbitMQIntegrationEventConsumer(
        ConnectionFactory connectionFactory,
        IOptions<RabbitMQConfiguration> configuration,
        IServiceScopeFactory serviceScopeFactory)
    {
        Console.WriteLine("RabbitMQIntegrationEventConsumer:Ctor");

        this.configuration = configuration.Value;
        this.serviceScopeFactory = serviceScopeFactory;

        persistentConnection = connectionFactory.CreateConnection();
    }

    public void Subscribe()
    {
        Console.WriteLine("RabbitMQIntegrationEventConsumer:Subscribe");

        var channel = persistentConnection.CreateModel();

        channel.ExchangeDeclare(configuration.IntegrationEventsExchangeName, ExchangeType.Topic, durable: true);

        var queueName = channel.QueueDeclare(nameof(RabbitMQIntegrationEventConsumer), durable: true, exclusive: false, autoDelete: false).QueueName;
        channel.QueueBind(queue: queueName, exchange: configuration.IntegrationEventsExchangeName, routingKey: "#");

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.Received += async (_, eventArgs) =>
        {
            var message = Encoding.UTF8.GetString(eventArgs.Body.Span);
            var integrationEvent = IntegrationEvent.JsonDeserialize(message);
            
            await using (var scope = serviceScopeFactory.CreateAsyncScope())
            {
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                await mediator.Publish(integrationEvent.ToNotification(), CancellationToken.None);
            }
        };

        channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
        persistentChannel = channel;
    }

    public void Dispose()
    {
        Console.WriteLine("RabbitMQIntegrationEventConsumer:Dispose");

        persistentChannel?.Dispose();
        persistentConnection?.Dispose();
    }
}