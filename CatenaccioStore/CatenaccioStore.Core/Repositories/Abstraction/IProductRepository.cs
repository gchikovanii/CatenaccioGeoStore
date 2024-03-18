using CatenaccioStore.Core.Entities;

namespace CatenaccioStore.Core.Repositories.Abstraction
{
    public interface IProductRepository
    {
        Task<Product> GetProductByIdAsync(CancellationToken token,int id);
        Task<IReadOnlyList<Product>> GetProductsAsync(CancellationToken token);
        Task<IReadOnlyList<ProductBrand>> GetProductBrandsAsync(CancellationToken token);
        Task<IReadOnlyList<ProductType>> GetProductTypesAsync(CancellationToken token);


    }
}
