using System.Collections.Generic;
using System.Text;
using Application;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Infrastructure;

public class RabbitMQIntegrationEventPublisher : IIntegrationEventPublisher
{
    private readonly ConnectionFactory connectionFactory;
    private readonly RabbitMQConfiguration configuration;

    public RabbitMQIntegrationEventPublisher(ConnectionFactory connectionFactory, IOptions<RabbitMQConfiguration> configuration)
    {
        this.connectionFactory = connectionFactory;
        this.configuration = configuration.Value;
    }

    public void Publish(IEnumerable<IntegrationEvent> integrationEvents)
    {
        using (var connection = connectionFactory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            channel.ExchangeDeclare(configuration.IntegrationEventsExchangeName, ExchangeType.Topic, durable: true);

            foreach (var integrationEvent in integrationEvents)
            {
                channel.BasicPublish(
                    exchange: configuration.IntegrationEventsExchangeName,
                    routingKey: nameof(IntegrationEvent),
                    mandatory: true,
                    basicProperties: null,
                    body: Encoding.UTF8.GetBytes(integrationEvent.JsonSerialize()));
            }
        }
    }
}