using CatenaccioStore.Core.Entities.Orders;

namespace CatenaccioStore.Core.Repositories.Abstraction
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsycn(CancellationToken token,string buyerEmail, int deliveryMethod, string basketId, Address shippingAdderss);
        Task<IReadOnlyList<Order>> GetOrdersForUserAscyn(CancellationToken token, string buyeEmail);
        Task<Order> GetOrderbyIdAscyn(CancellationToken token, int id, string buyerEmail);
        Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync(CancellationToken token);
    }
}
