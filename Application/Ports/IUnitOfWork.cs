using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application;

public interface IUnitOfWork : IAsyncDisposable
{
    Task BeginTransactionAsync(CancellationToken cancellationToken);
    Task CommitTransactionAsync(CancellationToken cancellationToken);
    bool HasOngoingTransaction { get; }
}