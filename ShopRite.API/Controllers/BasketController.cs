using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopRite.Platform.Basket;
using System.Threading.Tasks;

namespace ShopRite.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        public readonly IMediator _mediator;
        public BasketController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet("{basketId}")]
        public async Task<IActionResult> GetBasketById(string basketId)
        {
            var basket = await _mediator.Send(new GetBasket.Query { Id = basketId} );
            return Ok(basket);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateBasket(UpdateBasket.BasketUpdateRequest request)
        {
            var basket = await _mediator.Send(new UpdateBasket.Command { Request = request });
            if (basket.IsBasketUpdated is false) return StatusCode(StatusCodes.Status500InternalServerError);
            return Ok(basket);
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteBasket(DeleteBasket.BasketDeleteRequest request)
        {
            var basket = await _mediator.Send(new DeleteBasket.Command { Request = request });
            return Ok(basket);
        }
    }
}
