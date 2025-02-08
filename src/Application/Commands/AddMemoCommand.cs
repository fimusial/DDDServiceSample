using MediatR;

namespace Application;

public record AddMemoCommand : ICommand<Unit>
{
    required public string Content { get; init; }
}