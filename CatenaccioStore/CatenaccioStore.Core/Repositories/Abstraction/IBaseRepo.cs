using System.Linq.Expressions;


namespace CatenaccioStore.Core.Repositories.Abstraction
{
    public interface IBaseRepo<T> where T : class
    {
        Task<IEnumerable<T>> GetCollectionAsync(CancellationToken token, Expression<Func<T, bool>> expression = null);
        IQueryable<T> GetQuery(Expression<Func<T, bool>> expression = null);
        IQueryable<T> Table { get; }
        Task AddAsync(T entity, CancellationToken token);
        void Update(T entity, CancellationToken token);
        Task RemoveAsync(CancellationToken token, params object[] key);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken token);
        Task<bool> SaveChangesAsync(CancellationToken token);
    }
}
