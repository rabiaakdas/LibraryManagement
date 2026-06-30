using System.Threading.Tasks;
using LibraryManagement.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Web.Controllers.Api
{
    [ApiController]
    [Route("api/orders")]
    public class OrdersApiController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersApiController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _orderService.GetApiOrdersAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var order = await _orderService.GetApiOrderAsync(id);
            return order == null ? NotFound() : Ok(order);
        }
    }
}
