using AutoMapper;
using CatenaccioStore.API.DTOs;
using CatenaccioStore.Core.Entities;
using CatenaccioStore.Core.Repositories.Abstraction;
using CatenaccioStore.Core.Repositories.Specifications;

namespace CatenaccioStore.Infrastructure.Repositories.Implementation
{
    public class ProductRepository : IProductRepository
    {
        private readonly IRepository<Product> _productRepo;
        private readonly IRepository<ProductBrand> _productBrandRepo;
        private readonly IRepository<ProductType> _productTypeRepo;
        private readonly IMapper _mapper;
        public ProductRepository(IRepository<Product> productRepo, IRepository<ProductBrand> productBrandRepo, IRepository<ProductType> productTypeRepo, IMapper mapper)
        {
            _productRepo = productRepo;
            _productBrandRepo = productBrandRepo;
            _productTypeRepo = productTypeRepo;
            _mapper = mapper;
        }

        public async Task<IReadOnlyList<ProductDto>> GetProductsAsync(CancellationToken token, string sort)
        {
            var spec = new ProductsWithTypesAndBrandsSpecification(sort);
            var products = await _productRepo.ListAsync(token, spec).ConfigureAwait(false);
            return _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductDto>>(products);
        }

        public async Task<IReadOnlyList<ProductType>> GetProductTypesAsync(CancellationToken token)
        {
            return await _productTypeRepo.ListAllAsync(token).ConfigureAwait(false);
        }

        public async Task<IReadOnlyList<ProductBrand>> GetProductBrandsAsync(CancellationToken token)
        {
            return await _productBrandRepo.ListAllAsync(token).ConfigureAwait(false);
        }

        public async Task<ProductDto> GetProductByIdAsync(CancellationToken token, int id)
        {
            var spec = new ProductsWithTypesAndBrandsSpecification(id);
            var products = await _productRepo.GetEntityWithSpec(token, spec).ConfigureAwait(false);
            return _mapper.Map<Product, ProductDto>(products);
        }
    }
}
