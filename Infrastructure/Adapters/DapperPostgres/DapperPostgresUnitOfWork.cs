using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Application;
using Dapper;
using Npgsql;

namespace Infrastructure;

public class DapperPostgresUnitOfWork : IUnitOfWork, IAsyncDisposable
{
    private readonly NpgsqlConnection npgsqlConnection;
    private NpgsqlTransaction? currentTransaction;

    public DapperPostgresUnitOfWork(NpgsqlConnection npgsqlConnection)
    {
        Console.WriteLine("UnitOfWork:ctor");
        this.npgsqlConnection = npgsqlConnection;
    }

    public bool HasOngoingTransaction => currentTransaction is not null;

    public async Task BeginTransactionAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("UnitOfWork:BeginTransactionAsync");

        if (currentTransaction is not null)
        {
            throw new InvalidOperationException("this unit of work has already initiated a transaction");
        }

        await npgsqlConnection.OpenAsync(cancellationToken);
        currentTransaction = await npgsqlConnection.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
        await npgsqlConnection.ExecuteAsync(new CommandDefinition("SET TRANSACTION READ WRITE", cancellationToken: cancellationToken));
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("UnitOfWork:CommitTransactionAsync");

        if (currentTransaction is null)
        {
            throw new InvalidOperationException("cannot commit; a transaction has not been initiated");
        }

        await currentTransaction.CommitAsync(cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        Console.WriteLine("UnitOfWork:DisposeAsync");

        if (currentTransaction is not null)
        {
            await currentTransaction.DisposeAsync();
        }

        await npgsqlConnection.CloseAsync();
        await npgsqlConnection.DisposeAsync();

        Console.WriteLine();
    }
}