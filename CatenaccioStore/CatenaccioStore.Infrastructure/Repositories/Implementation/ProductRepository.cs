using CatenaccioStore.Core.Entities;
using CatenaccioStore.Core.Repositories.Abstraction;
using CatenaccioStore.Infrastructure.DataContext;
using Microsoft.EntityFrameworkCore;

namespace CatenaccioStore.Infrastructure.Repositories.Implementation
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<Product>> GetPoroductsAsync(CancellationToken token)
        {
            return await _context.Products.ToListAsync(token);
        }

        public async Task<Product> GetProductByIdAsync(CancellationToken token,int id)
        {
            return await _context.Products.FindAsync(id,token);
        }
    }
}
