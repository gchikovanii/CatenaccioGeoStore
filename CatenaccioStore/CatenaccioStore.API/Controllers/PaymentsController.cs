using CatenaccioStore.API.Errors;
using CatenaccioStore.Core.Entities;
using CatenaccioStore.Core.Entities.Orders;
using CatenaccioStore.Core.Repositories.Abstraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace CatenaccioStore.API.Controllers
{
    public class PaymentsController : BaseController
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentsController> _logger;
        private const string WhSecret = "whsec_a6595b314fdbab7ad1f643e699a2355c3850b1d599fb26ab3e4f390a301912fe";
        public PaymentsController(IPaymentService paymentService, ILogger<PaymentsController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }


        [Authorize]
        [HttpPost("{basketId}")]
        public async Task<ActionResult<CustomerBasket>> CreateOrUpdatePaymentIntent(CancellationToken token,string basketId)
        {
            var basket = await _paymentService.CreateOrUpdatePaymentIntent(token,basketId);
            if (basket == null)
                return BadRequest(new ApiResponse(400,"Problem With basket"));
            return basket;
        }
        [HttpPost("webhook")]
        public async Task<ActionResult> StripeWebHook(CancellationToken token)
        {
            var json = await new StreamReader(Request.Body).ReadToEndAsync();
            var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], WhSecret);
            PaymentIntent intent;
            Order order;

            switch (stripeEvent.Type)
            {
                case "payment_intent.succeeded":
                    intent = (PaymentIntent)stripeEvent.Data.Object;
                    order = await _paymentService.UpdateOrderPaymentSucceeded(token,intent.Id);
                    break;
                case "payment_intent.payment_failed":
                    intent = (PaymentIntent)stripeEvent.Data.Object;
                    order = await _paymentService.UpdateOrderPaymentFailed(token, intent.Id);
                    break;
                default:
                    break;
            }
            return new EmptyResult();
        }

    }
}
