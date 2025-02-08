using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application;

public interface IUnitOfWork
{
    bool HasOngoingTransaction { get; }

    Task BeginTransactionAsync(CancellationToken cancellationToken);

    Task CommitTransactionAsync(CancellationToken cancellationToken);
}