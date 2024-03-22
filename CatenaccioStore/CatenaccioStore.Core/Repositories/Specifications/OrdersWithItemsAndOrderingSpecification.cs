using CatenaccioStore.Core.Entities.Orders;
using System.Linq.Expressions;

namespace CatenaccioStore.Core.Repositories.Specifications
{
    public class OrdersWithItemsAndOrderingSpecification : BaseSpecification<Order>
    {
        public OrdersWithItemsAndOrderingSpecification(string email) : base(i => i.BuyerEmail == email)
        {
            AddInclude(i => i.OrderItems);  
            AddInclude(i => i.DeliveryMethod);
            AddOrderByDescending(i => i.OrderDate);
        }

        public OrdersWithItemsAndOrderingSpecification(int id, string email) : base(i => i.Id == id && i.BuyerEmail == email)
        {
            AddInclude(i => i.OrderItems);
            AddInclude(i => i.DeliveryMethod);
        }
    }
}
