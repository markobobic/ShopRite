using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopRite.Platform.Finance;
using System.Threading.Tasks;

namespace ShopRite.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FinanceController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FinanceController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet("per-month")]
        public async Task<IActionResult> GetFinancialStatistics([FromQuery] string month, int year)
        {
            var response = await _mediator.Send(new GetIncomePerMonth.Query { Month = month, Year = year });
            return Ok(response);
        }
        [HttpGet("per-year")]
        public async Task<IActionResult> GetFinancialStatistics([FromQuery] int year)
        {
            var response = await _mediator.Send(new GetIncomePerYear.Query { Year = year });
            return Ok(response);
        }
    }
}
