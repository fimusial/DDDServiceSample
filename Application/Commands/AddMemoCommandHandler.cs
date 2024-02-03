using System.Threading;
using System.Threading.Tasks;
using Domain;
using MediatR;

namespace Application;

public class AddMemoCommandHandler : IRequestHandler<AddMemoCommand, Unit>
{
    private readonly IMediator mediator;
    private readonly IRepository<Memo> memoRepository;

    public AddMemoCommandHandler(
        IMediator mediator,
        IRepository<Memo> memoRepository)
    {
        this.memoRepository = memoRepository;
        this.mediator = mediator;
    }

    public async Task<Unit> Handle(AddMemoCommand command, CancellationToken cancellationToken)
    {
        var memo = new Memo() { Content = command.Content };
        await memoRepository.AddAsync(memo, cancellationToken);
        await mediator.DispatchDomainEventsAsync(memo, cancellationToken);
        return Unit.Value;
    }
}