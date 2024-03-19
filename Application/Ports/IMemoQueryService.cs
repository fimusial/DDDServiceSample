using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application;

public interface IMemoQueryService
{
  Task<IEnumerable<int>> SearchMemoContentAsync(string term, CancellationToken cancellationToken);
}