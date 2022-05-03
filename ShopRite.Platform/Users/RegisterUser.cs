using Ardalis.GuardClauses;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Raven.Client.Documents.Session;
using ShopRite.Core.Extensions;
using ShopRite.Core.Interfaces;
using ShopRite.Domain;
using System.Threading;
using System.Threading.Tasks;

namespace ShopRite.Platform.Users
{
    public class RegisterUser
    {
        public class Command : IRequest<RegisterResponse>
        {
            public RegisterRequest RegisterRequest { get; set; }
        }
        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.RegisterRequest.Email).EmailAddress();
                RuleFor(x => x.RegisterRequest.Password).MinimumLength(6);
                RuleFor(x => x.RegisterRequest.FullName).NotEmpty();
            }
        }
        public class Handler : IRequestHandler<Command, RegisterResponse>
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
            public async Task<RegisterResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                var appUser = new AppUser
                {
                    Email = request.RegisterRequest.Email,
                    UserName = request.RegisterRequest.Username,
                    FullName = request.RegisterRequest.FullName,
                };
                var createUserResult = await  _userManager.CreateAsync(appUser, request.RegisterRequest.Password);
                Guard.Against.False(createUserResult.Succeeded, "Registration failed");
                await _userManager.AddToRoleAsync(appUser, AppUser.AdminRole);
                await _signInManager.SignInAsync(appUser, true);
                
                await _db.SaveChangesAsync();
                var token = _tokenService.CreateToken(appUser);
                return new RegisterResponse(token) { Email = request.RegisterRequest.Email, Username = request.RegisterRequest.Username };    
            }
        }
        public class RegisterResponse
        {
            public RegisterResponse(string token)
            {
                Token = token;
            }
            public string Email { get; set; }
            public string Username { get; set; }
            public string Token { get; set; }
        }
        public class RegisterRequest
        {
            public string FullName { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public string Username { get; set; }
        }
    }
}
