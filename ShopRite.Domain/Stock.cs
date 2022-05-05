using NUlid;

namespace ShopRite.Domain
{
    public record Stock 
    {
        public string Size { get; init; }
        public int Quantity { get; set; }
    }
}
