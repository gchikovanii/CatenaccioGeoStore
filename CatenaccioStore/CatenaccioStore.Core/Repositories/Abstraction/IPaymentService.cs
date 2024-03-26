using CatenaccioStore.Core.Entities;
using CatenaccioStore.Core.Entities.Orders;

namespace CatenaccioStore.Core.Repositories.Abstraction
{
    public interface IPaymentService
    {
        Task<CustomerBasket> CreateOrUpdatePaymentIntent(CancellationToken token, string basketId);
        Task<Order> UpdateOrderPaymentSucceeded(CancellationToken token, string paymentIntentId);
        Task<Order> UpdateOrderPaymentFailed(CancellationToken token, string paymentIntentId);
    }
}
