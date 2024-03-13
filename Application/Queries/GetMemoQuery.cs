using MediatR;

namespace Application;

public record GetMemoQuery : IRequest<string?>
{
    public int Id { get; set; }
}