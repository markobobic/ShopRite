using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ShopRite.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FinanceController : ControllerBase
    {
        public async Task<IActionResult> TotalIncomePerMonth()
        {
            return Ok();
        }
    }
}
