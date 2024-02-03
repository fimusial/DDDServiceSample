using System.Threading;
using System.Threading.Tasks;
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

    public Task AddAsync(Memo entity, CancellationToken cancellationToken)
    {
        return npgsqlConnection.ExecuteAsync(new CommandDefinition(
            "INSERT INTO Memo(content) VALUES(@Content)",
            parameters: entity,
            cancellationToken: cancellationToken
            ));
    }

    public Task<Memo> GetAsync(int id, CancellationToken cancellationToken)
    {
        return npgsqlConnection.QuerySingleAsync<Memo>(new CommandDefinition(
            "SELECT * FROM Memo WHERE id = @id",
            parameters: new { id },
            cancellationToken: cancellationToken
            ));
    }

    public Task UpdateAsync(Memo entity, CancellationToken cancellationToken)
    {
        return npgsqlConnection.ExecuteAsync(new CommandDefinition(
            "UPDATE Memo SET content = @Content WHERE id = @Id",
            parameters: entity,
            cancellationToken: cancellationToken
            ));
    }
}