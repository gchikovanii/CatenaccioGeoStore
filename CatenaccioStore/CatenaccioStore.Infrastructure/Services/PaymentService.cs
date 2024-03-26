using CatenaccioStore.Core.Entities;
using CatenaccioStore.Core.Entities.Orders;
using CatenaccioStore.Core.Repositories.Abstraction;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace CatenaccioStore.Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;

        public PaymentService(IBasketRepository basketRepository, IUnitOfWork unitOfWork, IConfiguration config)
        {
            _basketRepository = basketRepository;
            _unitOfWork = unitOfWork;
            _config = config;
        }

        public async Task<CustomerBasket> CreateOrUpdatePaymentIntent(CancellationToken token,string basketId)
        {
            StripeConfiguration.ApiKey = _config["StripeSettings:SecretKey"];
            var basket = await _basketRepository.GetBasketAsync(basketId);
            var shippingPrice = 0m;
            if (basket.DeliveryMethodId.HasValue)
            {
                var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(token,(int)basket.DeliveryMethodId);
                shippingPrice = deliveryMethod.Price;
            }
            foreach (var item in basket.BaksetItems)
            {
                var productItem = await _unitOfWork.Repository<Core.Entities.Product>().GetByIdAsync(token,item.Id);
                if(item.Price != productItem.Price)
                    item.Price = productItem.Price;
            }
            var service = new PaymentIntentService();
            PaymentIntent intent;
            if(string.IsNullOrEmpty(basket.PaymentIntentId))
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = (long)basket.BaksetItems.Sum(i => i.Quantity * (i.Price * 100)) + (long)shippingPrice * 100,
                    Currency = "usd",
                    PaymentMethodTypes = new List<string>
                    {
                        "card"
                    }
                };
                intent = await service.CreateAsync(options);
                basket.PaymentIntentId = intent.Id;
                basket.ClientSecret = intent.ClientSecret;
            }
            else
            {
                var options = new PaymentIntentUpdateOptions
                {
                    Amount = (long)basket.BaksetItems.Sum(i => i.Quantity * (i.Price * 100)) + (long)shippingPrice * 100
                };
                await service.UpdateAsync(basket.PaymentIntentId, options);
            }
            await _basketRepository.UpdateBasketAsync(basket);
            return basket;
        }
    }
}
