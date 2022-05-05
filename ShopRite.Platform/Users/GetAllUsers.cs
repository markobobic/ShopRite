using MediatR;
using Microsoft.AspNetCore.Identity;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using ShopRite.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ShopRite.Platform.Users
{
    public class GetAllUsers
    {
        public class Query : IRequest<List<UsersAllResponse>> { }
        public class QueryHandler : IRequestHandler<Query, List<UsersAllResponse>>
        {
            private readonly UserManager<AppUser> _userManager;

            public QueryHandler(UserManager<AppUser> userManager)
            {
                _userManager = userManager;
            }
            public async Task<List<UsersAllResponse>> Handle(Query request, CancellationToken cancellationToken)
            {
                var users = await _userManager.Users.ToListAsync();
                
                return users.Select(x => new UsersAllResponse
                {
                    Address = x?.Address?.FullAddress ?? string.Empty,
                    Email = x?.Email ?? string.Empty,
                    Role = x?.Roles?.FirstOrDefault(string.Empty) ,
                    Username = x?.UserName ?? string.Empty
                }).ToList();
            }
        }
        public class UsersAllResponse
        {
            public string Username { get; set; }
            public string Email { get; set; }
            public string Role { get; set; }
            public string Address { get; set; }
        }
    }
}
