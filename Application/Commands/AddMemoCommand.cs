using MediatR;

namespace Application;

public record AddMemoCommand : IRequest<Unit>
{
    public required string Content { get; set; }
}