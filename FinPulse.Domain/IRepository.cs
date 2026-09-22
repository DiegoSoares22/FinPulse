using System.Linq.Expressions;

namespace FinPulse.Repositories;

public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetAsync(Expression<Func<T, bool>> predicate);
    T Create(T entity);   // Create, Update e Delete NÃO precisam ser async
    T Update(T entity);   // porque são operações em memória (Change Tracker)
    T Delete(T entity);   // O async só é necessário quando há I/O (banco, rede, disco)
}