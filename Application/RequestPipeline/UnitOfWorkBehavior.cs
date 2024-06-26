using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Application;

public class UnitOfWorkBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICommand<TResponse>
{
    private readonly IUnitOfWork unitOfWork;

    public UnitOfWorkBehavior(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (unitOfWork.HasOngoingTransaction)
        {
            return await next();
        }

        await unitOfWork.BeginTransactionAsync(cancellationToken);
        var response = await next();
        await unitOfWork.CommitTransactionAsync(cancellationToken);

        return response;
    }
}