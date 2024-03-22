using CatenaccioStore.Core.Entities;
using CatenaccioStore.Core.Repositories.Specifications;

namespace CatenaccioStore.Core.Repositories.Abstraction
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task<T> GetByIdAsync(CancellationToken token,int id);
        Task<IReadOnlyList<T>> ListAllAsync(CancellationToken token);   
        Task<T> GetEntityWithSpec(CancellationToken token,ISpecification<T> specification);
        Task<IReadOnlyList<T>> ListAsync(CancellationToken token, ISpecification<T> specification);
        Task<int> CountAsync(CancellationToken token, ISpecification<T> specification);
        void Add(T entity); 
        void Update(T entity);
        void Delete(T entity);
    }
}
