using CatenaccioStore.Core.Entities;

namespace CatenaccioStore.Core.Repositories.Abstraction
{
    public interface IPaymentService
    {
        Task<CustomerBasket> CreateOrUpdatePaymentIntent(CancellationToken token, string basketId);
    }
}
