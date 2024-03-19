using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Application;

public class SearchMemoContentQueryHandler : IRequestHandler<SearchMemoContentQuery, IEnumerable<int>>
{
    private readonly IMemoQueryService memoQueryService;

    public SearchMemoContentQueryHandler(IMemoQueryService memoQueryService)
    {
        this.memoQueryService = memoQueryService;
    }

    public async Task<IEnumerable<int>> Handle(SearchMemoContentQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Term) || request.Term.Length > 10)
        {
            return Enumerable.Empty<int>();
        }

        var sanitizedTerm = new Regex("[^a-zA-Z0-9 ]").Replace(request.Term, string.Empty).Trim();

        return await memoQueryService.SearchMemoContentAsync(sanitizedTerm, cancellationToken);
    }
}