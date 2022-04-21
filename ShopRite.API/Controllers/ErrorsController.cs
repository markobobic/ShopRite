using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopRite.Core.Responses;

namespace ShopRite.API.Controllers
{
    [Route("errors/{code}")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [ApiController]
    public class ErrorsController : ControllerBase
    {
        public IActionResult Error(int code) => new ObjectResult(new ApiResponse(code));
    }
}
