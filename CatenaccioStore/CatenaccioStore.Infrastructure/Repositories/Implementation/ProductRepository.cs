using CatenaccioStore.Core.Entities;
using CatenaccioStore.Core.Repositories.Abstraction;
using CatenaccioStore.Core.Repositories.Specifications;
using CatenaccioStore.Infrastructure.DataContext;
using Microsoft.EntityFrameworkCore;

namespace CatenaccioStore.Infrastructure.Repositories.Implementation
{
    public class ProductRepository : IProductRepository
    {
        private readonly IRepository<Product> _productRepo;
        private readonly IRepository<ProductBrand> _productBrandRepo;
        private readonly IRepository<ProductType> _productTypeRepo;

        public ProductRepository(IRepository<Product> productRepo, IRepository<ProductBrand> productBrandRepo, IRepository<ProductType> productTypeRepo)
        {
            _productRepo = productRepo;
            _productBrandRepo = productBrandRepo;
            _productTypeRepo = productTypeRepo;
        }

        public async Task<IReadOnlyList<Product>> GetProductsAsync(CancellationToken token)
        {
            var spec = new ProductsWithTypesAndBrandsSpecification();
            return await _productRepo.ListAsync(token, spec).ConfigureAwait(false);
        }

        public async Task<IReadOnlyList<ProductType>> GetProductTypesAsync(CancellationToken token)
        {
            return await _productTypeRepo.ListAllAsync(token).ConfigureAwait(false);
        }

        public async Task<IReadOnlyList<ProductBrand>> GetProductBrandsAsync(CancellationToken token)
        {
            return await _productBrandRepo.ListAllAsync(token).ConfigureAwait(false);
        }

        public async Task<Product> GetProductByIdAsync(CancellationToken token, int id)
        {
            var spec = new ProductsWithTypesAndBrandsSpecification(id);
            return await _productRepo.GetEntityWithSpec(token, spec).ConfigureAwait(false);
        }
    }
}
