using System.Threading;
using System.Threading.Tasks;
using Domain;
using MediatR;

namespace Application;

public class AddMemoCommandHandler : IRequestHandler<AddMemoCommand, Unit>
{
    private readonly IMediator mediator;
    private readonly IRepository<Memo> memoRepository;

    public AddMemoCommandHandler(IMediator mediator, IRepository<Memo> memoRepository)
    {
        this.memoRepository = memoRepository;
        this.mediator = mediator;
    }

    public async Task<Unit> Handle(AddMemoCommand request, CancellationToken cancellationToken)
    {
        System.Console.WriteLine("adding a memo!");

        var memo = new Memo() { Content = request.Content };
        var memoId = await memoRepository.AddAsync(memo, cancellationToken);
        memo.MemoCreated(memoId);
        await mediator.DispatchDomainEventsAsync(memo, cancellationToken);
        return Unit.Value;
    }
}