using CatenaccioStore.Core.Entities;

namespace CatenaccioStore.Core.Repositories.Abstraction
{
    public interface IProductRepository
    {
        Task<Product> GetProductByIdAsync(CancellationToken token,int id);
        Task<IReadOnlyList<Product>> GetPoroductsAsync(CancellationToken token);

    }
}
