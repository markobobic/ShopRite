using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopRite.Platform.Stocks;
using System.Threading.Tasks;

namespace ShopRite.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StocksController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StocksController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost]
        public async Task<IActionResult> CreateStocks(CreateStock.StockCreateDTO request)
        {
            await _mediator.Send(new CreateStock.Command(request));
            return Ok();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStocks(string id)
        {
            await _mediator.Send(new DeleteStock.Command(id));
            return NoContent();
        }
        [HttpGet]
        public async Task<IActionResult> GetAllStocks()
        {
            var stocks = await _mediator.Send(new GetStocks.Query());
            return Ok(stocks);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateStocks(UpdateStock.Request request)
        {
            var updatedStocks = await _mediator.Send(new UpdateStock.Command(request));
            return Ok(updatedStocks);
        }
    }
}
