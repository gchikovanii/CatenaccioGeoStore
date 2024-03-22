using AutoMapper;
using CatenaccioStore.API.Errors;
using CatenaccioStore.API.Infrastructure.Extensions;
using CatenaccioStore.Core.DTOs;
using CatenaccioStore.Core.Entities.Orders;
using CatenaccioStore.Core.Repositories.Abstraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CatenaccioStore.API.Controllers
{
    [Authorize]
    public class OrdersController : BaseController
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;
        public OrdersController(IOrderService orderService, IMapper mapper)
        {
            _orderService = orderService;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<OrderToReturnDto>>> GetOrdersForUser(CancellationToken token)
        {
            var email = HttpContext.User.RetriveEmailFromPrincipal();
            var orders = await _orderService.GetOrdersForUserAscyn(token,email);
            return Ok(_mapper.Map<IReadOnlyList<OrderToReturnDto>>(orders));
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderToReturnDto>> GetOrderByIdForUser(CancellationToken token, int id)
        {
            var email = HttpContext.User.RetriveEmailFromPrincipal();
            var order = await _orderService.GetOrderbyIdAscyn(token,id, email);
            if (order == null)
                return NotFound(new ApiResponse(404));
            return Ok(_mapper.Map<OrderToReturnDto>(order));
        }
        [HttpGet("DeliveryMethod")]
        public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethods(CancellationToken token)
        {
            return Ok(await _orderService.GetDeliveryMethodsAsync(token));
        }
        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder(CancellationToken token, OrderDto orderDto)
        {
            var email = HttpContext.User.RetriveEmailFromPrincipal();
            var address = _mapper.Map<AddressDto, Address>(orderDto.ShipToAddress);
            var order = await _orderService.CreateOrderAsycn(token,email, orderDto.DeliveryMethodId, orderDto.BasketId, address);
            if (order == null)
                return BadRequest(new ApiResponse(400, "Problem while making order"));
            return Ok(order);
        }
    }
}
