using System.Collections.Generic;
using MediatR;

namespace Application;

public class SearchMemoContentQuery : IRequest<IEnumerable<int>>
{
    required public string Term { get; init; }
}