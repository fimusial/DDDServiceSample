using MediatR;

namespace Application;

public interface ICommand<out TResponse> : IRequest<TResponse>
{
}