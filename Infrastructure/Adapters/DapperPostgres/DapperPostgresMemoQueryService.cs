using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application;
using Dapper;
using Npgsql;

namespace Infrastructure;

public class DapperPostgresMemoQueryService : IMemoQueryService
{
    private readonly NpgsqlConnection npgsqlConnection;

    public DapperPostgresMemoQueryService(NpgsqlConnection npgsqlConnection)
    {
        this.npgsqlConnection = npgsqlConnection;
    }

    public Task<IEnumerable<int>> SearchMemoContentAsync(string term, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(term, nameof(term));

        return npgsqlConnection.QueryAsync<int>(new CommandDefinition(
            "SELECT id FROM memo WHERE LOWER(content) LIKE LOWER(@Term) ORDER BY id DESC LIMIT 5",
            parameters: new { Term = $"%{term}%" },
            cancellationToken: cancellationToken));
    }
}