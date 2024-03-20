using CatenaccioStore.Core.Entities;

namespace CatenaccioStore.Core.Repositories.Abstraction
{
    public interface IBasketRepository
    {
        Task<CustomerBasket> GetBasketAsync(string basketId);
        Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket);
        Task<bool> DeleteBasketAsync(string basketId);
        
    }
}
