using MediatR;

namespace Application;

public record AddMemoCommand : ICommand<Unit>
{
    public required string Content { get; set; }
}