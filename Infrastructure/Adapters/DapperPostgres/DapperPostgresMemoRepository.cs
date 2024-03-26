using System.Threading;
using System.Threading.Tasks;
using Application;
using Dapper;
using Domain;
using Npgsql;

namespace Infrastructure;

public class DapperPostgresMemoRepository : IRepository<Memo>
{
    private readonly NpgsqlConnection npgsqlConnection;

    public DapperPostgresMemoRepository(NpgsqlConnection npgsqlConnection)
    {
        this.npgsqlConnection = npgsqlConnection;
    }

    public Task<int> AddAsync(Memo entity, CancellationToken cancellationToken)
    {
        return npgsqlConnection.ExecuteScalarAsync<int>(new CommandDefinition(
            "INSERT INTO memo(content) VALUES(@Content) RETURNING id",
            parameters: entity,
            cancellationToken: cancellationToken));
    }

    [AllowWithoutTransaction]
    public Task<Memo?> GetAsync(int id, CancellationToken cancellationToken)
    {
        return npgsqlConnection.QuerySingleOrDefaultAsync<Memo>(new CommandDefinition(
            "SELECT * FROM memo WHERE id = @id",
            parameters: new { id },
            cancellationToken: cancellationToken));
    }

    public Task UpdateAsync(Memo entity, CancellationToken cancellationToken)
    {
        return npgsqlConnection.ExecuteAsync(new CommandDefinition(
            "UPDATE memo SET content = @Content WHERE id = @Id",
            parameters: entity,
            cancellationToken: cancellationToken));
    }
}