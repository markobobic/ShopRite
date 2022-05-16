using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopRite.Platform.Finance;
using System.Threading.Tasks;

namespace ShopRite.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReportsController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet("per-month")]
        public async Task<IActionResult> GetFinancialStatisticsPerMonth([FromQuery] string month, int year) => 
            Ok(await _mediator.Send(new GetIncomePerMonth.Query { Month = month, Year = year }));

        [HttpGet("per-year")]
        public async Task<IActionResult> GetFinancialStatisticsPerYear([FromQuery] int year) =>
            Ok(await _mediator.Send(new GetIncomePerYear.Query { Year = year }));
    }
}
