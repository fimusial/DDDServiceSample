using System.Threading;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application;

public class AddMemoCommandHandler : IRequestHandler<AddMemoCommand, Unit>
{
    private readonly ILogger<AddMemoCommandHandler> logger;
    private readonly IMediator mediator;
    private readonly IRepository<Memo> memoRepository;

    public AddMemoCommandHandler(
        ILogger<AddMemoCommandHandler> logger,
        IMediator mediator,
        IRepository<Memo> memoRepository)
    {
        this.logger = logger;
        this.memoRepository = memoRepository;
        this.mediator = mediator;
    }

    public async Task<Unit> Handle(AddMemoCommand request, CancellationToken cancellationToken)
    {
        logger.LogHandlerRunning(nameof(AddMemoCommandHandler));

        var memo = new Memo() { Content = request.Content };
        var memoId = await memoRepository.AddAsync(memo, cancellationToken);
        memo.MemoCreated(memoId);
        await mediator.DispatchDomainEventsAsync(memo, cancellationToken);
        return Unit.Value;
    }
}