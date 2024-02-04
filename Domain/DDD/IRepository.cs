using System.Threading;
using System.Threading.Tasks;

namespace Domain;

public interface IRepository<T>
    where T : Entity
{
    Task<T> GetAsync(int id, CancellationToken cancellationToken);
    Task<int> AddAsync(T entity, CancellationToken cancellationToken);
    Task UpdateAsync(T entity, CancellationToken cancellationToken);
}