using CatenaccioStore.Core.Entities;
using CatenaccioStore.Core.Repositories.Abstraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CatenaccioStore.API.Controllers
{
    public class PaymentsController : BaseController
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }


        [Authorize]
        [HttpPost("{basketId}")]
        public async Task<ActionResult<CustomerBasket>> CreateOrUpdatePaymentIntent(CancellationToken token,string basketId)
        {
            return await _paymentService.CreateOrUpdatePaymentIntent(token,basketId);
        }

    }
}
