using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Raven.Client.Documents.Session;
using ShopRite.Domain;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace ShopRite.Platform.Users
{
    public class GetCurrentUser
    {
        public class Query : IRequest<UserResponse> { }
        public class QueryHandler : IRequestHandler<Query, UserResponse>
        {
            private readonly UserManager<AppUser> _userManager;
            private readonly IHttpContextAccessor _context;
            public QueryHandler(UserManager<AppUser> userManager, IHttpContextAccessor context)
            {
                _userManager = userManager;
                _context = context;
            }

            public async Task<UserResponse> Handle(Query request, CancellationToken cancellationToken)
            {
                var email = _context.HttpContext.User.FindFirst(ClaimTypes.Email).Value ?? string.Empty;
                var currentUser = await _userManager.FindByEmailAsync(email);
                return new UserResponse
                {
                    Address = currentUser?.Address?.FullAddress ?? string.Empty,
                    Email = currentUser?.Email ?? string.Empty,
                    FullName = currentUser?.FullName ?? string.Empty,
                    Username = currentUser?.UserName ?? string.Empty,
                };
            }
        }
        public class UserResponse
        {
            public string Email { get; set; }
            public string Username { get; set; }
            public string Address { get; set; }
            public string FullName { get; set; }
        }
    }
}
