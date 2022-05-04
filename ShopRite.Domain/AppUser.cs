namespace ShopRite.Domain
{
    public class AppUser : Raven.Identity.IdentityUser
    {
        public const string AdminRole = "Admin";
        public const string BuyerRole = "Buyer";
        public string FullName { get; set; }
        public Address Address { get; set; }
    }
}
