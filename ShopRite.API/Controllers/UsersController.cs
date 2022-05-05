using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MediatR;
using ShopRite.Platform.Users;
using ShopRite.Core.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using ShopRite.Core.Constants;

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
        public async Task<IActionResult> RegisterAsync(RegisterUser.RegisterRequest request)
        {
            var response = await _mediator.Send(new RegisterUser.Command { RegisterRequest = request });
            if (!response.IsSuccessful) return BadRequest(response.RegistrationErrors);
            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(LoginUser.LoginRequest request)
        {
            var response = await _mediator.Send(new LoginUser.Command { LoginRequest = request });
            if (!response.UserExist || !response.SignInResult) return Unauthorized(new ApiResponse(401));
            return Ok(response.UserSuccessResponse);
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = UserRole.Admin)]
        [HttpGet]
        public async Task<IActionResult> GetUsersAsync()
        {
            var users = await _mediator.Send(new GetAllUsers.Query());
            return Ok(users);
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = UserRole.Admin)]
        [HttpGet("currentUser")]
        public async Task<IActionResult> GetCurrentUserAsync()
        {
            var user = await _mediator.Send(new GetCurrentUser.Query());
            if(user == null) return NotFound();
            return Ok(user);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateUserAsync(UpdateUser.UpdateUserRequest request)
        {
            var response = await _mediator.Send(new UpdateUser.Command { UpdateUserRequest = request });
            if (response.DoesUserExist is false) return NotFound("User is not found.");
            if (!response.IsPasswordChanged) return BadRequest("Something went wrong.");
            return Ok(response.UserUpdateDto);
        }

    }
}
