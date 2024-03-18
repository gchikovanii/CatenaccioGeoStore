using CatenaccioStore.API.DTOs;
using CatenaccioStore.Core.Entities;
using CatenaccioStore.Core.Repositories.Specifications;
using CatenaccioStore.Infrastructure.Helpers;

namespace CatenaccioStore.Core.Repositories.Abstraction
{
    public interface IProductRepository
    {
        Task<ProductDto> GetProductByIdAsync(CancellationToken token,int id);
        Task<Pagination<ProductDto>> GetProductsAsync(CancellationToken token, ProductSpecParams productSpecParams);
        Task<IReadOnlyList<ProductBrand>> GetProductBrandsAsync(CancellationToken token);
        Task<IReadOnlyList<ProductType>> GetProductTypesAsync(CancellationToken token);


    }
}
