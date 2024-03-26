using CatenaccioStore.Core.Entities.Orders;

namespace CatenaccioStore.Core.Repositories.Specifications
{
    public class OrderByPaymentIntentIdSpecification : BaseSpecification<Order>
    {
        public OrderByPaymentIntentIdSpecification(string paymentIntentId) : base(i => i.PaymentIntentId == paymentIntentId)
        {
        }
    }
}
