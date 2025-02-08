using MediatR;

namespace Application;

public class PublishIntegrationEventsCommand : ICommand<Unit>
{
    public int BatchSize { get; init; }
}