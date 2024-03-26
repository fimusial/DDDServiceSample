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

    public async Task<IEnumerable<int>> SearchMemoContentAsync(string term, CancellationToken cancellationToken)
    {
        return await npgsqlConnection.QueryAsync<int>(new CommandDefinition(
            "SELECT id FROM memo WHERE LOWER(content) LIKE LOWER(@Term) ORDER BY id DESC LIMIT 5",
            parameters: new { Term = $"%{term}%" },
            cancellationToken: cancellationToken));
    }
}