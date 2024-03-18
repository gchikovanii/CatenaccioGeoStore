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

        public async Task<IReadOnlyList<Product>> GetProductsAsync(CancellationToken token)
        {
            return await _context.Products.Include(i => i.ProductType).Include(i => i.ProductBrand).ToListAsync(token);
        }

        public async Task<IReadOnlyList<ProductType>> GetProductTypesAsync(CancellationToken token)
        {
            return await _context.ProductTypes.ToListAsync(token);
        }

        public async Task<IReadOnlyList<ProductBrand>> GetProductBrandsAsync(CancellationToken token)
        {
            return await _context.ProductBrands.ToListAsync(token);
        }

        public async Task<Product> GetProductByIdAsync(CancellationToken token,int id)
        {
            return await _context.Products
                .Include(i => i.ProductType).Include(i => i.ProductBrand)
                .FirstOrDefaultAsync(i => i.Id == id,token);
        }
    }
}
