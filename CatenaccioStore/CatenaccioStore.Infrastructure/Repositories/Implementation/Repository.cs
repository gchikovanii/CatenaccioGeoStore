using CatenaccioStore.Core.Entities;
using CatenaccioStore.Core.Repositories.Abstraction;
using CatenaccioStore.Core.Repositories.Specifications;
using CatenaccioStore.Infrastructure.DataContext;
using Microsoft.EntityFrameworkCore;

namespace CatenaccioStore.Infrastructure.Repositories.Implementation
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly ApplicationDbContext _context;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<T> GetByIdAsync(CancellationToken token, int id)
        {
            return await _context.Set<T>().FindAsync(id, token);
        }
        public async Task<IReadOnlyList<T>> ListAllAsync(CancellationToken token)
        {
            return await _context.Set<T>().ToListAsync(token);
        }
        public async Task<IReadOnlyList<T>> ListAsync(CancellationToken token, ISpecification<T> specification)
        {
            return await ApplySpecification(specification).ToListAsync(token);
        }
        public async Task<T> GetEntityWithSpec(CancellationToken token, ISpecification<T> specification)
        {
            return await ApplySpecification(specification).FirstOrDefaultAsync(token);
        }

        private IQueryable<T> ApplySpecification(ISpecification<T> specification)
        {
            return SpecificationEvaluator<T>.GetQuery(_context.Set<T>().AsQueryable(), specification);
        }

    }
}
