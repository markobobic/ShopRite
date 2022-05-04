using ShopRite.Domain;

namespace ShopRite.Core.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(AppUser user);

    }
}
