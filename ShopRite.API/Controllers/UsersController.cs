using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MediatR;
using ShopRite.Platform.Users;

namespace ShopRite.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        public readonly IMediator _mediator;
        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUser.RegisterRequest request)
        {
            var response = await _mediator.Send(new RegisterUser.Command { RegisterRequest = request });
            return Ok(response);
        }
    }
}
