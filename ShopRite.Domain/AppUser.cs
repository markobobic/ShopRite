namespace ShopRite.Domain
{
    public class AppUser : Raven.Identity.IdentityUser
    {
        public string FullName { get; set; }
        public Address Address { get; set; } 
    }
}
