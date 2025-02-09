using MediatR;

namespace Application;

public record PublishIntegrationEventsCommand : ICommand<Unit>
{
    public int BatchSize { get; init; }
}