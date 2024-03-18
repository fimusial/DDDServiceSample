namespace Infrastructure;

public class RabbitMQConfiguration
{
    required public string HostName { get; set; }

    public int Port { get; set; }

    required public string Username { get; set; }

    required public string Password { get; set; }

    required public string IntegrationEventsExchangeName { get; set; }

    required public string IntegrationEventsDeadLetterExchangeName { get; set; }
}