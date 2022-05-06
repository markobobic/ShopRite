using Coravel.Mailer.Mail.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopRite.Core.Services;
using ShopRite.Platform.Orders;
using System.Threading.Tasks;

namespace ShopRite.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost]
        public async Task<IActionResult> PostOrder(CreateOrder.CreateOrderRequest request)
        {
           var response = await _mediator.Send(new CreateOrder.Command { CreateOrderRequest = request });
           return Ok(response);
        }
    }
}
