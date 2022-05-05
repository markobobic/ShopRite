using MediatR;
using Microsoft.AspNetCore.Identity;
using Raven.Client.Documents.Session;
using ShopRite.Domain;
using System.Threading;
using System.Threading.Tasks;

namespace ShopRite.Platform.Users
{
    public class UpdateUser
    {
        public class Command : IRequest<UpdateUserResponse>
        {
            public UpdateUserRequest UpdateUserRequest { get; set; }
        }

        public class Handler : IRequestHandler<Command, UpdateUserResponse>
        {
            private readonly UserManager<AppUser> _userManager;
            private readonly IAsyncDocumentSession _db;

            public Handler(UserManager<AppUser> userManager, IAsyncDocumentSession db)
            {
                _userManager = userManager;
                _db = db;
            }
            public async Task<UpdateUserResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _userManager.FindByIdAsync(request.UpdateUserRequest.Id);
                if (user is not null)
                {
                    user.Address = request.UpdateUserRequest.Address;
                    user.UserName = request.UpdateUserRequest.Username;
                    var changePasswordResult = await _userManager
                                                .ChangePasswordAsync(user, request.UpdateUserRequest.CurrentPassword, request.UpdateUserRequest.NewPassword);
                    if (changePasswordResult.Succeeded)
                    {
                        await _userManager.UpdateAsync(user);
                        await _db.SaveChangesAsync(cancellationToken);
                        
                        return new UpdateUserResponse
                        {
                            DoesUserExist = true,
                            IsPasswordChanged = true,
                            UserUpdateDto  = new UserUpdateDto
                            {
                                Address = user.Address,
                                Username = user.UserName
                            }
                        };
                    }
                }
                return new UpdateUserResponse
                {
                    DoesUserExist = user != null,
                    IsPasswordChanged = false,
                };
            }
        }

        public class UpdateUserResponse
        {
            public bool DoesUserExist { get; set; }
            public bool IsPasswordChanged { get; set; }
            public UserUpdateDto UserUpdateDto { get; set; }
            
        }
        public class UserUpdateDto
        {
            public string Username { get; set; }
            public Address Address { get; set; }
        }
        public class UpdateUserRequest
        {
            public string Id { get; set; }
            public string Username { get; set; }
            public string CurrentPassword { get; set; }
            public string NewPassword { get; set; }
            public Address Address { get; set; }

        }
    }


}
