using MediatR;
using Microsoft.AspNetCore.Identity;
using Raven.Client.Documents.Session;
using ShopRite.Core.Interfaces;
using ShopRite.Domain;
using System.Threading;
using System.Threading.Tasks;

namespace ShopRite.Platform.Users
{
    public class LoginUser
    {
        public class Command : IRequest<LoginResponse>
        {
            public LoginRequest LoginRequest { get; set; }
        }
        public class Handler : IRequestHandler<Command, LoginResponse>
        {
            private readonly IAsyncDocumentSession _db;
            private readonly UserManager<AppUser> _userManager;
            private readonly SignInManager<AppUser> _signInManager;
            private readonly ITokenService _tokenService;

            public Handler(IAsyncDocumentSession db,
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ITokenService tokenService)
            {
                _db = db;
                _userManager = userManager;
                _signInManager = signInManager;
                _tokenService = tokenService;
            }
            public async Task<LoginResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _userManager.FindByEmailAsync(request.LoginRequest.Email);
                var signInResult = await _signInManager.PasswordSignInAsync(user ?? new AppUser(), request.LoginRequest.Password, true, false);
                
                return new LoginResponse
                {
                    SignInResult = signInResult.Succeeded,
                    UserExist = user == null ? false : true,
                    UserSuccessResponse = new UserDto()
                    {
                        Email = request.LoginRequest.Email,
                        FullName = user?.FullName ?? string.Empty,
                        Token = user == null ? string.Empty : _tokenService.CreateToken(user),
                    }
                };
            }
        }
        public class LoginResponse
        {
            public bool SignInResult { get; set; }
            public bool UserExist { get; set; }
            public UserDto UserSuccessResponse { get; set; }
            
        }
        public class UserDto
        {
            public string FullName { get; set; }
            public string Email { get; set; }
            public string Token { get; set; }
        }
        public class LoginRequest
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }
    }
}
