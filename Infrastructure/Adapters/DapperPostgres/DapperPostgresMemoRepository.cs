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
    private readonly IUnitOfWork unitOfWork;

    public DapperPostgresMemoRepository(
        NpgsqlConnection npgsqlConnection,
        IUnitOfWork unitOfWork)
    {
        this.npgsqlConnection = npgsqlConnection;
        this.unitOfWork = unitOfWork;
    }

    public Task<int> AddAsync(Memo entity, CancellationToken cancellationToken)
    {
        unitOfWork.ThrowIfNoOngoingTransaction();

        return npgsqlConnection.ExecuteScalarAsync<int>(new CommandDefinition(
            "INSERT INTO Memo(content) VALUES(@Content) RETURNING id",
            parameters: entity,
            cancellationToken: cancellationToken
            ));
    }

    public Task<Memo?> GetAsync(int id, CancellationToken cancellationToken)
    {
        return npgsqlConnection.QuerySingleOrDefaultAsync<Memo>(new CommandDefinition(
            "SELECT * FROM Memo WHERE id = @id",
            parameters: new { id },
            cancellationToken: cancellationToken
            ));
    }

    public Task UpdateAsync(Memo entity, CancellationToken cancellationToken)
    {
        unitOfWork.ThrowIfNoOngoingTransaction();

        return npgsqlConnection.ExecuteAsync(new CommandDefinition(
            "UPDATE Memo SET content = @Content WHERE id = @Id",
            parameters: entity,
            cancellationToken: cancellationToken
            ));
    }
}