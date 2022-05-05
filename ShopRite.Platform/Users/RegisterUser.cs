using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Raven.Client.Documents.Session;
using ShopRite.Core.Enumerations;
using ShopRite.Core.Interfaces;
using ShopRite.Domain;
using System.Collections.Generic;
using System.Linq;
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
                
                RuleFor(x => x.RegisterRequest.UserRole).Must(ub =>
                {
                    UserRoles.TryFromValue(ub, out var userRole);
                    return userRole != null && userRole != string.Empty;
                })
                    .WithMessage("User role is not valid!");
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
                if (createUserResult.Succeeded)
                {
                    await _userManager.AddToRoleAsync(appUser, request.RegisterRequest.UserRole);
                    await _signInManager.SignInAsync(appUser, true);

                    await _db.SaveChangesAsync();
                    var token = _tokenService.CreateToken(appUser);
                    return new RegisterResponse(token) { IsSuccessful = true, Email = request.RegisterRequest.Email, Username = request.RegisterRequest.Username };
                }
                return new RegisterResponse(string.Empty) 
                {
                    IsSuccessful = false, 
                    Email = string.Empty, 
                    Username = string.Empty, 
                    Address = request.RegisterRequest.Address,
                    RegistrationErrors = createUserResult.Errors.Select(x => x.Description).ToList() 
                };
            }
        }
        public class RegisterResponse
        {
            public RegisterResponse(string token)
            {
                Token = token;
            }
            public bool IsSuccessful { get; set; }
            public string Email { get; set; }
            public string Username { get; set; }
            public string Token { get; set; }
            public Address Address { get; set; }
            public List<string> RegistrationErrors { get; set; }
        }
        public class RegisterRequest
        {
            public string FullName { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public string Username { get; set; }
            public string UserRole { get; set; }
            public Address Address { get; set; }
        }
    }
}
