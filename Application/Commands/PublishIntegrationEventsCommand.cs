using MediatR;

namespace Application;

public class PublishIntegrationEventsCommand : IRequest<Unit>
{
    public int BatchSize { get; set; }
}