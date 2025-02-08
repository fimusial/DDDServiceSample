using System.Threading;
using System.Threading.Tasks;
using Domain;
using MediatR;

namespace Application;

public class GetMemoQueryHandler : IRequestHandler<GetMemoQuery, string?>
{
    private readonly IRepository<Memo> memoRepository;

    public GetMemoQueryHandler(IRepository<Memo> memoRepository)
    {
        this.memoRepository = memoRepository;
    }

    public async Task<string?> Handle(GetMemoQuery request, CancellationToken cancellationToken)
    {
        var memo = await memoRepository.GetAsync(request.Id, cancellationToken);
        return memo?.Content;
    }
}