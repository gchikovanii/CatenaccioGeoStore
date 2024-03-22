using CatenaccioStore.Core.Entities;
using CatenaccioStore.Core.Entities.Orders;
using CatenaccioStore.Core.Repositories.Abstraction;
using CatenaccioStore.Core.Repositories.Specifications;

namespace CatenaccioStore.Infrastructure.Repositories.Implementation
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBasketRepository _basketRepository;
        public OrderService(IUnitOfWork unitOfWork, IBasketRepository basketRepository)
        {
            _unitOfWork = unitOfWork;
            _basketRepository = basketRepository;
        }
        public async Task<Order> CreateOrderAsycn(CancellationToken token, string buyerEmail, int deliveryMethodId, string basketId, Address shippingAdderss)
        {
            var basket = await _basketRepository.GetBasketAsync(basketId);
            var items = new List<OrderItem>();
            foreach (var item in basket.BaksetItems)
            {
                var productItem = await _unitOfWork.Repository<Product>().GetByIdAsync(token,item.Id);
                var itemOrdered = new ProductItemOrdered(productItem.Id, productItem.Name, productItem.PictureUrl);
                var orderItem = new OrderItem(itemOrdered, productItem.Price, item.Quantity);
                items.Add(orderItem);
            }

            var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(token, deliveryMethodId);
            var subTotal = items.Sum(i => i.Price * i.Quantity);
            var order = new Order(items,buyerEmail,shippingAdderss, deliveryMethod,subTotal);
            _unitOfWork.Repository<Order>().Add(order);
            var result = await _unitOfWork.Complete();
            if (result <= 0)
                return null;
            await _basketRepository.DeleteBasketAsync(basketId);
            return order;
        }

        public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync(CancellationToken token)
        {
            return await _unitOfWork.Repository<DeliveryMethod>().ListAllAsync(token);
        }

        public async Task<Order> GetOrderbyIdAscyn(CancellationToken token, int id, string buyerEmail)
        {
            var spec = new OrdersWithItemsAndOrderingSpecification(id, buyerEmail);
            return await _unitOfWork.Repository<Order>().GetEntityWithSpec(token, spec);
        }

        public async Task<IReadOnlyList<Order>> GetOrdersForUserAscyn(CancellationToken token, string buyeEmail)
        {
            var spec = new OrdersWithItemsAndOrderingSpecification(buyeEmail);
            return await _unitOfWork.Repository<Order>().ListAsync(token,spec);
        }
    }
}
